using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MinLength(0), MaxLength(12)]
        public string CCCD { get; set; }
        [MinLength(0), MaxLength(50)]
        public string Address { get; set; }
        [Required]
        [MinLength(0), MaxLength(50)]
        public string FullName { get; set; }
        public double SoDuTaiKhoan { get; set; }
    }
}
