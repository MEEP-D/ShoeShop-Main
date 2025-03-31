using System.ComponentModel.DataAnnotations;

namespace ShoeShop.ViewModels.Authentication
{
	public class ResetPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		[StringLength(100, ErrorMessage = "{0} phải dài ít nhất {2} ký tự.", MinimumLength = 6)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$", ErrorMessage = "Mật khẩu phải chứa ít nhất một chữ cái thường, một chữ cái viết hoa, một chữ số và một ký tự không phải chữ và số.")]
		public string Password { get; set; }
		[DataType(DataType.Password)]
		[Display(Name = "Confirm Password")]
		[Compare("Password", ErrorMessage = "Mật khẩu không khớp!")]
		public string ConfirmPassword { get; set; }
		public string Code { get; set; }
	}
}
