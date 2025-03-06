using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Repositories.Interfaces;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Services
{
  public class SoTietKiemService : ISoTietKiemService
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SoTietKiemService> _logger;
    private readonly DapperContext _dapperContext;
    private readonly ISoTietKiemRepository _soTietKiemRepository;

    public SoTietKiemService(ApplicationDbContext context,
        ILogger<SoTietKiemService> logger, DapperContext dapperContext, ISoTietKiemRepository soTietKiemRepository)
    {
      _context = context;
      _logger = logger;
      _dapperContext = dapperContext;
      _soTietKiemRepository = soTietKiemRepository;
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
    public async Task<double> GetSoDuSoTietKiemByCodeSTK(string userId, string CodeSTK)
    {
      var soDuSoTietKiem = await _context.SoTietKiems.Where(s => s.UserId == userId && s.Code == CodeSTK).Select(s => s.SoDuSoTietKiem).FirstOrDefaultAsync();
      return (double)soDuSoTietKiem;
    }
    public async Task<int> GetCountSoTietKiemInMonth(string userId)
    {
      return await _soTietKiemRepository.GetCountSoTietKiemInMonthAsync(userId);
    }

  }
}