using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Entity; 
  public class LoaiGiaoDich
  {
    [Key]
    public int MaLoaiGiaoDich {get;set;} // 
    [Required]
    [StringLength(100)]
    public string TenLoaiGiaoDich {get;set;} // Tên loại giao dịch 
    //Ví dụ: 
    // Giao dịch rút tiền
    // Giao dịch gửi tiền
    // Giao dịch chuyển tiền
    // Giao dịch nạp tiền
    public virtual ICollection<GiaoDich>? GiaoDichs {get;set;}
  }

