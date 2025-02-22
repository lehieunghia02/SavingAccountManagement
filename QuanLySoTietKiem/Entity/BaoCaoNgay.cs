using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models
{
    public class BaoCaoNgay
    {
        [Key]
        public int MaBaoCaoNgay { get; set; }

        [Required]
        public int MaLoaiSo { get; set; }
        [Required]
        public DateTime Ngay { get; set; }

        [Required]
        public decimal TongTienGui { get; set; }

        [Required]
        public decimal TongTienRut { get; set; }
        [Required]
        public DateTime NgayTaoBaoCao { get; set; }
        [ForeignKey("MaLoaiSo")]
        public virtual LoaiSoTietKiem? LoaiSoTietKiem { get; set; }
    }
}
