using System.ComponentModel.DataAnnotations;

namespace ShoeShop.ViewModels.Authentication
{
	public class RegisterViewModel
	{

		[Display(Name = "Full name")]
		[Required(ErrorMessage = "Địa chỉ tên đầy đủ là bắt buộc")]
		public string FullName { get; set; }

		[Display(Name = "User name")]
		[Required(ErrorMessage = "Địa chỉ tên người dùng là bắt buộc")]
		public string UserName { get; set; }

		[Display(Name = "Email address")]
		[Required(ErrorMessage = "Địa chỉ email là bắt buộc")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		public string EmailAddress { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[MinLength(8, ErrorMessage = "Mật khẩu phải dài ít nhất 8 ký tự")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Mật khẩu phải chứa ít nhất một chữ cái thường, một chữ cái hoa và một chữ số")]
		public string Password { get; set; }
		 
		[Display(Name = "Confirm password")]
		[Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Mật khẩu không khớp")]
		public string ConfirmPassword { get; set; }
	}
}
