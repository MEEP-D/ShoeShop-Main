using ShoeShop.Models;
using System;
using System.ComponentModel.DataAnnotations;

public class CustomerViewModel
{
    [Display(Name = "Tên đầy đủ")]
    [Required(ErrorMessage = "Tên đầy đủ là bắt buộc")]
    public string FullName { get; set; }

    [Display(Name = "UserName")]
    [Required(ErrorMessage = "Tên là bắt buộc")]
    public string UserName { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }

    [Display(Name = "Phone")]
    [Required(ErrorMessage = "SDT là bắt buộc")]
    public string PhoneNumber { get; set; }

    [Display(Name = "BirthDay")]
    [Required(ErrorMessage = "BirthDay là bắt buộc")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime BirthDay { get; set; }

    [Display(Name = "Password")]
    [Required(ErrorMessage = "Password là bắt buộc")]
    public string Password { get; set; }

    [Display(Name = "Gender")]
    [Required(ErrorMessage = "Gender là bắt buộc")]
    public int Gender { get; set; }

    [Display(Name = "Status")]
    [Required(ErrorMessage = "Status là bắt buộc")]
    public bool Status { get; set; }
}
