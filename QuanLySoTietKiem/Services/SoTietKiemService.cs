using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Services
{
    public class SoTietKiemService : ISoTietKiemService
    {
      private readonly ApplicationDbContext _context;
      public SoTietKiemService(ApplicationDbContext context)
      {
        _context = context;
      }
        public async Task<int> CountSoTietKiem(string userId)
        {
          var count = await _context.SoTietKiems.Where(s => s.UserId == userId).CountAsync();
          return count;
        }
        public async Task<string> GetCodeSTK(string userId, int maSoTietKiem)
        {
        return await _context.SoTietKiems
          .Where(s => s.UserId == userId && s.MaSoTietKiem == maSoTietKiem)
          .Select(s => s.Code)
          .FirstOrDefaultAsync() ?? string.Empty;
        }
        public async Task<double> GetSoDuSoTietKiemByCodeSTK(string userId, string CodeSTK){          
          var soDuSoTietKiem = await _context.SoTietKiems.Where(s => s.UserId == userId && s.Code == CodeSTK).Select(s => s.SoDuSoTietKiem).FirstOrDefaultAsync();
          return (double)soDuSoTietKiem;
        }
    }
}