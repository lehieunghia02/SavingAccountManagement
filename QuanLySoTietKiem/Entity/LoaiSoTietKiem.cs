using System.ComponentModel.DataAnnotations;


namespace QuanLySoTietKiem.Models
{
    public class LoaiSoTietKiem
    {
        [Key]
        public int MaLoaiSo { get; set; }

        [Required]
        public string? TenLoaiSo { get; set; } // Example: "Ky Han 12 Thang"
        [Required]
        public double LaiSuat { get; set; } // Example: 10% = 0.1
        [Required]
        public int KyHan { get; set; } // Example: 12
        [Required]
        public int ThoiGianGuiToiThieu { get; set; } // Example: 1
        [Required]
        public decimal SoTienGuiToiThieu { get; set; } //Example: 10000000

        //Navigation Properties

        public virtual ICollection<BaoCaoNgay>? BaoCaoNgays { get; set; }
        public virtual ICollection<BaoCaoThang>? BaoCaoThangs { get; set; }
        public virtual ICollection<SoTietKiem>? SoTietKiems { get; set; }

    }
}
