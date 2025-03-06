using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Repositories.Interfaces;

namespace QuanLySoTietKiem.Repositories.Implementations;

public class SoTietKiemRepository : ISoTietKiemRepository
{
  private readonly ApplicationDbContext _context;
  
  public SoTietKiemRepository(ApplicationDbContext context)
  {
    _context = context;
  }
  public async Task<int> GetCountSoTietKiemInMonthAsync(string userId)
  {
    var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
    var endDate = startDate.AddMonths(1);
    var count = await _context.SoTietKiems.Where(x => x.UserId == userId && x.NgayMoSo >= startDate && x.NgayMoSo < endDate).CountAsync();
    return count;
  }
}