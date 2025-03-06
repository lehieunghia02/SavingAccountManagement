namespace QuanLySoTietKiem.Repositories.Interfaces;

public interface ISoTietKiemRepository
{
  public Task<int> GetCountSoTietKiemInMonthAsync(string userId);

}