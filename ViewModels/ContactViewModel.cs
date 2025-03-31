using System.ComponentModel.DataAnnotations;

namespace ShoeShop.ViewModels
{
	public class ContactViewModel
	{
		[Required(ErrorMessage = "Tên là bắt buộc")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Email là bắt buộc")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Tin nhắn là bắt buộc")]
		public string Message { get; set; }
	}

}
