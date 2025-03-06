using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySoTietKiem.Entity
{
    public class SoTietKiem
    {
        [Key]
        public int MaSoTietKiem { get; set; } // 1
        [Required]
        public string? Code { get; set; } // STK001, STK002, STK003
        [Required]
        public int MaLoaiSo { get; set; }
        [Required]
        public int MaHinhThucDenHan { get; set; }
        [Required]
        public decimal SoDuSoTietKiem { get; set; } // So du so tiet kiem
        [Required]
        public decimal SoTienGui { get; set; } // Số tiền gửi
        [Required]
        public decimal LaiSuatKyHan { get; set; } // Lãi suất kỳ hạn
        [Required]
        public bool TrangThai { get; set; } // Trạng thái
        [Required]
        public decimal LaiSuatApDung { get; set; } // Lãi suất áp dụng
        [Required]
        public DateTime NgayMoSo { get; set; } // Ngày mở sổ
        public DateTime? NgayDongSo { get; set; } // Ngày đóng sổ
        [Required]
        public DateTime NgayDaoHan { get; set; } // Ngày đáo hạn
        [Required]
        public string UserId { get; set; }
        [ForeignKey("MaLoaiSo")]
        public virtual LoaiSoTietKiem? LoaiSoTietKiem { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
        [ForeignKey("MaHinhThucDenHan")]
        public virtual HinhThucDenHan? HinhThucDenHan { get; set; }
        public virtual ICollection<GiaoDich>? GiaoDichs { get; set; }
    }
}
