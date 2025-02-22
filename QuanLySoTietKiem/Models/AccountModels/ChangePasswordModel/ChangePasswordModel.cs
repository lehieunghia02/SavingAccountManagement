using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.AccountModels.ChangePasswordModel;

public class ChangePasswordModel
{
    [Required]
    [DataType(DataType.Password)]
    [DisplayName("Current Password")]
    public string CurrentPassword { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [DisplayName("New Password")]
    public string NewPassword { get; set; }
    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string? ConfirmNewPassword { get; set; }
}