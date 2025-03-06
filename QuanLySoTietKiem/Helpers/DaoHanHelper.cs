using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;

namespace QuanLySoTietKiem.Helpers;
public static class DaoHanHelper
{
  public static async Task XuLyDaoHan(SoTietKiem soTietKiem, decimal tienLai, ApplicationDbContext context)
  {
    var user = await context.Users.FindAsync(soTietKiem.UserId);
    if (user == null)
      throw new Exception("Không tìm thấy thông tin người dùng");

    switch (soTietKiem.MaHinhThucDenHan)
    {
      case 1: // Rút hết
        await RutHet(soTietKiem, tienLai, user, context);
        break;
      case 2: // Quay vòng gốc
        await QuayVongGoc(soTietKiem, tienLai, user, context);
        break;
      case 3: // Quay vòng cả gốc và lãi
        await QuayVongGocVaLai(soTietKiem, tienLai, context);
        break;
      default:
        throw new ArgumentException("Hình thức đáo hạn không hợp lệ");
    }
  }

  private static async Task RutHet(SoTietKiem soTietKiem, decimal tienLai, ApplicationUser user, ApplicationDbContext context)
  {

    // Cộng tiền gốc và lãi vào tài khoản
    user.SoDuTaiKhoan += (double)(soTietKiem.SoDuSoTietKiem + tienLai);

    soTietKiem.NgayDongSo = DateTime.Now;
    soTietKiem.SoDuSoTietKiem = 0;
    soTietKiem.TrangThai = false;

    // Lưu thay đổi
    context.Users.Update(user); //cập nhật số dư tài khoản 
    context.SoTietKiems.Update(soTietKiem);  // cập nhật số dư STK 
    await context.SaveChangesAsync();
  }
  //Quay vòng gốc là rút lãi và tạo kỳ hạn mới với số tiền gốc ban đầu 
  private static async Task QuayVongGoc(SoTietKiem soTietKiem, decimal tienLai, ApplicationUser user, ApplicationDbContext context)
  {

    // Cộng tiền lãi vào tài khoản (chỉ rút lãi)
    user.SoDuTaiKhoan += (double)tienLai;

    // Tạo kỳ hạn mới với số tiền gốc ban đầu
    soTietKiem.NgayMoSo = DateTime.Now;
    var kyHan = soTietKiem.LoaiSoTietKiem.KyHan; 
    soTietKiem.NgayDaoHan = soTietKiem.NgayMoSo.AddMonths(kyHan);
    soTietKiem.SoDuSoTietKiem = soTietKiem.SoTienGui; // set lại số dư sổ tiết kiệm là số tiền gửi ban đầu 

    // Lưu thay đổi
    context.Users.Update(user);
    context.SoTietKiems.Update(soTietKiem);
    await context.SaveChangesAsync();
  }

  private static async Task QuayVongGocVaLai(SoTietKiem soTietKiem, decimal tienLai, ApplicationDbContext context)
  {
    //Tạo kỳ hạn mới
    soTietKiem.NgayMoSo = DateTime.Now;
    soTietKiem.NgayDaoHan = soTietKiem.NgayMoSo.AddMonths(soTietKiem.LoaiSoTietKiem.KyHan);
    //Số dư = gốc + lãi 
    soTietKiem.SoDuSoTietKiem += tienLai;
    soTietKiem.SoTienGui = soTietKiem.SoDuSoTietKiem;

    // Lưu lại vào database
    context.SoTietKiems.Update(soTietKiem);
    await context.SaveChangesAsync();
  }
}