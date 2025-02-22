using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.AccountModels.RegisterrModel
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long"), 
        MaxLength(50, ErrorMessage = "Username must be less than 50 characters long")]
        public string UserName { get; set; } = string.Empty; 
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty; 
        [Required(ErrorMessage = "Fullname is required")]
        [MinLength(3, ErrorMessage = "Fullname must be at least 3 characters long"),
        MaxLength(50, ErrorMessage = "Fullname must be less than 50 characters long")]
        public string FullName { get; set; } = string.Empty; 
        [Required(ErrorMessage = "Address is required")]
        [MinLength(3, ErrorMessage = "Address must be at least 3 characters long"),
        MaxLength(50, ErrorMessage = "Address must be less than 50 characters long")]
        public string Address { get; set; } = string.Empty; 
        [MinLength(9, ErrorMessage = "CCCD must be at least 9 characters long"),
        MaxLength(12, ErrorMessage = "CCCD must be less than 12 characters long")]
        [Required(ErrorMessage = "CCCD is required")]
        public string CCCD { get; set; } = string.Empty; 
        public double SoDuTaiKhoan { get; set; } = 100000000;
        [Required(ErrorMessage = "Phone number is required")]
        [MinLength(10, ErrorMessage = "Phone number must be at least 10 characters long"),
        MaxLength(11, ErrorMessage = "Phone number must be less than 11 characters long")]
        public string PhoneNumber { get; set; } = string.Empty; 
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long"),
        MaxLength(50, ErrorMessage = "Password must be less than 50 characters long")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
