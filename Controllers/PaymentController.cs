using Microsoft.AspNetCore.Mvc;
using ShoeShop.Models;
using PayPal.v1.Payments;
using ShoeShop.Services;
using ShoeShop.ViewModels;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Data;
using PayPal.v1.Invoices;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using ShoeShop.Hubs;
using ShoeShop.Library;
using ShoeShop.Data.Enum;
using PayPal.v1.Orders;

namespace ShoeShop.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPayPalService _payPalService;
		private readonly AppDbContext _context;
		private readonly IHubContext<OrderHub> _orderHubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IVnPayService _vnPayService;
        private readonly IConfiguration _config;

        public PaymentController(AppDbContext context, IPayPalService payPalService, IHubContext<OrderHub> orderHubContext, IHttpContextAccessor httpContextAccessor, IVnPayService vnPayService, IConfiguration config)
        {
			_context = context;
            _payPalService = payPalService;
			_orderHubContext = orderHubContext;
            _httpContextAccessor = httpContextAccessor;
            _vnPayService = vnPayService;
            _config = config;

        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] PaymentViewModel paymentInfo)
        {
            var cartVariantSizeIds = paymentInfo.Cart.Select(ci => ci.VariantSizeId).ToList();
            var variantSize = await _context.VariantSizes
                .Where(v => cartVariantSizeIds.Contains(v.Id))
                .Select(v => new OrderDetail
                {
                    VariantSizeId = v.Id,
                    Price = v.Variant.Product.PriceSale != 0 ? v.Variant.Product.PriceSale : v.Variant.Product.Price,
                })
                .ToListAsync();
            foreach (var item in variantSize)
            {
                item.Quantity = paymentInfo.Cart.First(ci => ci.VariantSizeId == item.VariantSizeId).Quantity;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (paymentInfo.AddressId == -1)
            {
                paymentInfo.NewAddress.AppUserId = userId;
                _context.Add(paymentInfo.NewAddress);
                await _context.SaveChangesAsync();
                paymentInfo.AddressId = paymentInfo.NewAddress.Id;
            }

            var shippingMethod = await _context.ShippingMethods.FindAsync(paymentInfo.ShippingMethodId);
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(x => x.Code == paymentInfo.VoucherCode);
            decimal subTotal = variantSize.Sum(v => v.Quantity * v.Price);
            int? IdVoucher = null;
            decimal discountAmount = 0;
            if (voucher != null && voucher.Number >= 0 && voucher.Status)
            {
                voucher.Number -= 1;
                discountAmount = voucher.Type ? (subTotal * ((decimal)voucher.Discount / 100m)) : (subTotal - (decimal)voucher.Discount);
                _context.Update(voucher);
                IdVoucher = voucher.Id;
            }
            var order = new Models.Order
            {
                AppUserId = userId,
                ShippingMethodId = paymentInfo.ShippingMethodId,
                PaymentMethod = paymentInfo.PaymentMethodId,
                SubTotal = subTotal - discountAmount,
                ShippingFee = shippingMethod.Cost,
                Description = paymentInfo.OrderDescription,
                OrderStatus = 0,
                Discount = discountAmount,
                Details = variantSize,
                AddressId = paymentInfo.AddressId,
                VoucherId = IdVoucher
            };

            _context.Add(order);
			await _context.SaveChangesAsync();
            await _orderHubContext.Clients.All.SendAsync("ReceiveOrderUpdate");

            if (paymentInfo.PaymentMethodId == 0)
            {
                TempData["OrderId"] = order.Id;
                return Json($"https://localhost:7107/Payment/PayPalReturn?payment_method=COD&success=1&order_id={order.Id}");
            }
            if (paymentInfo.PaymentMethodId == 2)
            {

                var vnPaymentRequest = new VnPaymentRequestModel
                {
                    OrderId = order.Id,
                    Amount = (double)(subTotal - discountAmount + shippingMethod.Cost) * 25520,
                    CreatedDate = DateTime.Now,
                    Description = paymentInfo.OrderDescription,
                    FullName = paymentInfo.NewAddress.FullName
                };
                string paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, vnPaymentRequest);
                return Json(paymentUrl);
            }
            else
            {
                var url = await _payPalService.CreatePaymentUrl(order, HttpContext);
                return Json(url);
            }
        }

        [Route("checkout/success")]
        public IActionResult PaymentSuccess()
        {
            if (TempData["OrderId"] != null)
            {
                int orderId = int.Parse(TempData["OrderId"].ToString());
                TempData.Keep("OrderId");
                var order = _context.Orders
                    .Where(o => o.Id == orderId)
                    .Select(o => new
                    {
                        o.Id,
                        ShippingMethod = o.ShippingMethod.Name,
                        PaymentMethod = o.PaymentMethod == 0 ? "Cash on delivery" :
                            o.PaymentMethod == 1 ? "Payment with Paypal" :
                            "Payment with VNPAY",
                        o.SubTotal,
                        o.ShippingFee,
                        o.Description,
                        o.PaymentStatus,
                        o.OrderStatus,
                        o.Address,
                        o.Discount,
                        VoucherCode = o.Voucher.Code,
                        Details = o.Details.Select(p => new
                        {
                            VariantSizeId = p.VariantSizeId,
                            ProductId = p.VariantSize.Variant.Product.Id,
                            ProductSlug = p.VariantSize.Variant.Product.Slug,
                            Name = p.VariantSize.Variant.Product.Name,
                            Thumbnail = p.VariantSize.Variant.Product.Thumbnail.Name,
                            Size = p.VariantSize.Size.Name,
                            Color = p.VariantSize.Variant.Color.Name,
                            p.Price,
                            p.Quantity,
                        }).ToList()
                    }).FirstOrDefault();
                if (order != null)
                {
                    ViewBag.Order = order;
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> PayPalReturn(string payment_method, int success, int order_id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == order_id);
            if (success == 1)
            {
                if(payment_method == "PayPal")
                {

                    if (order != null)
                    {
                        order.PaymentStatus = true;
                        await _context.SaveChangesAsync();
                    }
                }
                TempData["OrderId"] = order.Id;
                return RedirectToAction("PaymentSuccess");
            }
            else
            {
                order.OrderStatus = OrderStatus.Canceled;
                order.PaymentStatus = false;
                await _context.SaveChangesAsync();
                return RedirectToAction("PaymentFailed", new { txnRef = order_id });
            }
        }
        [HttpGet]
        public async Task<IActionResult> PaymentCallBack()
        {
            var vnpayData = Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());

            string vnp_SecureHash = vnpayData["vnp_SecureHash"];
            vnpayData.Remove("vnp_SecureHash");

            var sortedParams = vnpayData.OrderBy(k => k.Key, StringComparer.Ordinal)
                                        .Select(kv => $"{kv.Key}={kv.Value}")
                                        .ToList();
            string signData = string.Join("&", sortedParams);
            string hashSecret = _config["VnPay:HashSecret"];
            string calculatedHash = Utils.HmacSHA512(hashSecret, signData);

            if (calculatedHash != vnp_SecureHash)
            {
            }

            string transactionStatus = vnpayData["vnp_TransactionStatus"];
            string txnRef = vnpayData["vnp_TxnRef"];

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id.ToString() == txnRef);
            if (order == null)
            {
                return BadRequest("Không tìm thấy đơn hàng!");
            }

            if (transactionStatus == "00") 
            {
                order.OrderStatus = OrderStatus.Unconfirmed;
                order.PaymentStatus = true; 
                await _context.SaveChangesAsync();
                TempData["OrderId"] = txnRef;
                return RedirectToAction("PaymentSuccess");
            }
            else 
            {
                order.PaymentStatus = false;
                order.OrderStatus = OrderStatus.Canceled; 
                await _context.SaveChangesAsync();
                return RedirectToAction("PaymentFailed", new { orderInfo = order.Description, txnRef = txnRef });
            }
        }
        [Route("checkout/failed")]
        public IActionResult PaymentFailed(string txnRef)
        {
            ViewBag.TxnRef = txnRef;
            return View();
        }
    }
}
