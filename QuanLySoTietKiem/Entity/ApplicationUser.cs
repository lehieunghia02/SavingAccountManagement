using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace QuanLySoTietKiem.Entity
{
    public class ApplicationUser : IdentityUser
    {
        [MinLength(0), MaxLength(12)]
        public string CCCD { get; set; }
        [MinLength(0), MaxLength(50)]
        public string Address { get; set; }
        [Required]
        [MinLength(0), MaxLength(50)]
        public string FullName { get; set; } = string.Empty;
        public double SoDuTaiKhoan { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
