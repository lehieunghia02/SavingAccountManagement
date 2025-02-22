using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.SoSanhGoiTietKiem
{
  public class SoSanhGoiTietKiemModel
  {
    [Required(ErrorMessage = "Vui lòng nhập số tiền")]
    [Range(1000000, double.MaxValue, ErrorMessage = "Số tiền tối thiểu là 1.000.000đ")]
    public decimal SoTienDuKienGui { get; set; }

    public List<ChiTietGoiTietKiemModel> DanhSachGoi { get; set; } = new();
  }

  public class ChiTietGoiTietKiemModel
  {
    public int KyHan { get; set; }
    public decimal LaiSuat { get; set; }
    public decimal TienLaiDuKien { get; set; }
    public decimal TongTienNhanDuoc { get; set; }
    public string MoTa { get; set; }
    public List<string> UuDiem { get; set; } = new();
    public List<string> NhuocDiem { get; set; } = new();
  }
}

