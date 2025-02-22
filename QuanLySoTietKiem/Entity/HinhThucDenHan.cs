using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models;

public class HinhThucDenHan
{
    [Key]
    public int MaHinhThucDenHan { get; set; }
    [Required]
    [MinLength(1), MaxLength(50)]
    public string? TenHinhThucDenHan { get; set; }

    public virtual ICollection<SoTietKiem>? SoTietKiems { get; set; }
}