using System.ComponentModel.DataAnnotations;

namespace ShoeShop.ViewModels
{
	public class ReviewViewModel
	{
		[Required(ErrorMessage = "ProductId là bắt buộc")]
		public int ProductId { get; set; }
		[Required(ErrorMessage = "Description là bắt buộc")]
		public string Description { get; set; }
		[Required(ErrorMessage = "Rating là bắt buộc")]
		public int Rating { get; set; }
	}
}
