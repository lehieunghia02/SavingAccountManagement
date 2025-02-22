using QuanLySoTietKiem.Models;

namespace QuanLySoTietKiem.Repositories.Interfaces
{
  public interface ILaiSuatDuKienRepository
  {
    Task<decimal> GetLaiSuatByKyHanAsync(int kyHan);
  }
}

