using System.Diagnostics;

namespace QuanLySoTietKiem.Helpers;
public static class LaiSuatHelper
{
  public const decimal LAI_SUAT_KHONG_KY_HAN = 0.5m;
  //Tính lãi suất gửi tiền

  public static decimal TinhTienLai(decimal soTienGui, decimal laiSuat, int soNgayGui)
  {
    var result = soTienGui * (laiSuat / 100) * (soNgayGui / 365m);
    return result;
  }

  //Tính lãi suất rút tiền
  //ngayMoSo: Ngày mở sổ tiết kiệm 
  //ngayDaoHan: Ngày đáo hạn của sổ tiết kiệm 
  //ngayRut: Ngày khách hàng rút tiền 
  //laiSuatKyHan: Lãi suất kỳ hạn
  // public static decimal TinhLaiSuatRutTien(DateTime ngayMoSo, DateTime ngayDaoHan, DateTime ngayRut, decimal laiSuatKyHan)
  // {
  //   if (ngayRut < ngayMoSo)
  //   {
  //     throw new Exception("Ngày rút tiền không được nhỏ hơn ngày mở sổ tiết kiệm");
  //   }
  //   if (ngayDaoHan != ngayRut)
  //   {
  //     //int soNgayQuaHan = DaoHanHelper.TinhSoNgayQuaHan(ngayMoSo, ngayDaoHan, ngayRut);
  //     return LAI_SUAT_KHONG_KY_HAN;
  //   }
  //   // Tính số ngày đã gửi
  //   int soNgayGui = (ngayRut - ngayMoSo).Days;
  //   int tongSoNgayKyHan = (ngayDaoHan - ngayMoSo).Days;

  //   // Rút đúng ngày đáo hạn
  //   if (ngayRut.Date == ngayDaoHan.Date)
  //   {
  //     return laiSuatKyHan; // được hưởng đúng lãi suất kỳ hạn
  //   }
  //   // Rút sau ngày đáo hạn
  //   else if (ngayRut > ngayDaoHan)
  //   {
  //     int soNgayQuaHan = (ngayRut - ngayDaoHan).Days;
  //     return laiSuatKyHan + (LAI_SUAT_KHONG_KY_HAN * soNgayQuaHan / 365); // được hưởng lãi suất không kỳ hạn
  //   }
  //   // Rút trước hạn
  //   else
  //   {
  //     return LAI_SUAT_KHONG_KY_HAN; // được hưởng lãi suất không kỳ hạn
  //   }

  //   //   double tyLeThoiGianDaGui = soNgayGui / (double)tongSoNgayKyHan;
  //   //   if (tyLeThoiGianDaGui < 1.0 / 3) // Dưới 1/3 kỳ hạn
  //   //   {
  //   //     return LAI_SUAT_KHONG_KY_HAN;
  //   //   }
  //   //   else if (tyLeThoiGianDaGui < 2.0 / 3) // Từ 1/3 đến 2/3 kỳ hạn
  //   //   {
  //   //     return laiSuatKyHan * 0.3m;
  //   //   }
  //   //   else // Trên 2/3 kỳ hạn
  //   //   {
  //   //     return laiSuatKyHan * 0.5m;
  //   //   }
  //   // }
  // }

  //Tính lãi suất rút tiền
  public static decimal TinhLaiSuatRutTien(DateTime ngayMoSo, DateTime ngayDaoHan, DateTime ngayRut, decimal laiSuatKyHan)
  {
    if (ngayRut < ngayMoSo)
    {
      throw new Exception("Ngày rút tiền không được nhỏ hơn ngày mở sổ tiết kiệm");
    }
    // Rút đúng ngày đáo hạn
    else if (ngayRut.Date == ngayDaoHan.Date)
    {
      Debug.WriteLine("Case: Exact maturity date withdrawal");
      return laiSuatKyHan;
    }
    else
    {
      Debug.WriteLine("Case: Late withdrawal");
      return LAI_SUAT_KHONG_KY_HAN;
    }

  }

}