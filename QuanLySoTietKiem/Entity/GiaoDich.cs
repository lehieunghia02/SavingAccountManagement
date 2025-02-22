using System.ComponentModel.DataAnnotations;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySoTietKiem.Entity;
public class GiaoDich {
  [Key]
  public int MaGiaoDich { get; set; }
  [Required]
  public int MaSoTietKiem {get;set;}
  [Required]
  public int MaLoaiGiaoDich {get; set;}
  
  [Required]
  public DateTime NgayGiaoDich {get;set;}
  [Required]
  public double SoTien {get;set;}
  
  
  // Mối quan hệ giữa các bảng 
  //Mối quan hệ giữa bảng GiaoDich và bảng số tiết kiệm 
  [ForeignKey("MaLoaiGiaoDich")]
  public virtual LoaiGiaoDich? LoaiGiaoDich {get;set;}
  // Mối quan hệ giữa bảng GiaoDich và bảng SoTietKiem
  [ForeignKey("MaSoTietKiem")]
  public virtual SoTietKiem? SoTietKiem {get;set;}

}