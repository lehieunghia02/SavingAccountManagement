using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySoTietKiem.Models
{
    public class SoTietKiemModel
    {
        public int MaSoTietKiem { get; set; }
        [Required]
        public string? Code { get; set; }
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }
        [Required]
        public int MaLoaiSo { get; set; }
        [Required]
        public int MaHinhThucDenHan { get; set; }
        public decimal SoDuSoTietKiem { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền gửi phải lớn hơn 0")]
        public decimal SoTienGui { get; set; } // Số tiền gửi 
        public decimal LaiSuatKyHan { get; set; }  // Lãi suất kỳ hạn 
        public bool TrangThai { get; set; } // Trạng thái của sổ 
        public decimal LaiSuatApDung { get; set; } // Lãi suất áp dụng 
        public DateTime NgayMoSo { get; set; } // Ngày mở sổ 
        public DateTime NgayDongSo { get; set; } // Ngày đóng sổ 
        public DateTime NgayDaoHan { get; set; } // Ngày đáo hạn 
        public string? TenLoaiSo { get; set; }
        public int KyHan { get; set; }

        public string? TenHinhThucDenHan { get; set; }




    }
}