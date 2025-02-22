using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.AccountModels.LoginModel
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long"), 
        MaxLength(50, ErrorMessage = "Username must be less than 50 characters long")]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long"),
        MaxLength(50, ErrorMessage = "Password must be less than 50 characters long")]
        public string Password { get; set; } = string.Empty;
    }
}
