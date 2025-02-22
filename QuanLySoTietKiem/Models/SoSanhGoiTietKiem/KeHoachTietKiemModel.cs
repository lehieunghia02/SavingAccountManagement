using System.ComponentModel.DataAnnotations;

public class KeHoachTietKiemModel
{
  [Required(ErrorMessage = "Vui lòng nhập mục tiêu tiết kiệm")]
  [Range(1000000, double.MaxValue, ErrorMessage = "Mục tiêu tối thiểu là 1.000.000đ")]
  public decimal MucTieuTietKiem { get; set; }

  [Required(ErrorMessage = "Vui lòng nhập thời gian")]
  [Range(1, 60, ErrorMessage = "Thời gian từ 1-60 tháng")]
  public int ThoiGianThang { get; set; }

  public decimal SoTienGuiHangThang { get; set; }
  public decimal TongTienLai { get; set; }
  public List<ChiTietKeHoachModel> ChiTietKeHoach { get; set; } = new();
}

public class ChiTietKeHoachModel
{
  public int Thang { get; set; }
  public decimal SoTienGui { get; set; }
  public decimal LaiSuatThang { get; set; }
  public decimal TienLaiThang { get; set; }
  public decimal TongTichLuy { get; set; }
  public decimal TyLeDatMucTieu { get; set; }
}