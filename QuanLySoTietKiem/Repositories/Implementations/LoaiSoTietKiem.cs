using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Repositories.Interfaces;

namespace QuanLySoTietKiem.Repositories.Implementations
{
  public class LoaiSoTietKiemRepository : ILoaiSoTietKiemRepository
  {
    private readonly ApplicationDbContext _context;
    public LoaiSoTietKiemRepository(ApplicationDbContext context)
    {
      _context = context;
    }
    public async Task<IEnumerable<LoaiSoTietKiem>> GetAllLoaiSoTietKiemAsync()
    {
      return await _context.LoaiSoTietKiems.ToListAsync();
    }
  }
}

