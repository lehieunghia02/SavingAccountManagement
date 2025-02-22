using QuanLySoTietKiem.Models;

namespace QuanLySoTietKiem.Repositories.Interfaces
{
  public interface ILoaiSoTietKiemRepository
  {
    Task<IEnumerable<LoaiSoTietKiem>> GetAllLoaiSoTietKiemAsync();
  }
}
