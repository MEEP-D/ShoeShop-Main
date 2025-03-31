using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShoeShop.ViewModels
{
    public class UserProfileViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người dùng")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Tên người dùng phải có từ 3 đến 20 ký tự")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đầy đủ")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đầy đủ phải từ 3 đến 50 ký tự")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập địa chỉ email hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Vui lòng nhập số điện thoại hợp lệ")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        public int Gender { get; set; }

        public IFormFile? ImageFile { get; set; }
    }

}
