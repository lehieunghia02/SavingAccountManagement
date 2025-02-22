using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Models.AccountModels.ChangePasswordModel;
using QuanLySoTietKiem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Data;

namespace QuanLySoTietKiem.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISoTietKiemService _soTietKiemService;
        private readonly ApplicationDbContext _context;

        public UserController(ILogger<UserController> logger, UserManager<ApplicationUser> userManager, ISoTietKiemService soTietKiemService, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _soTietKiemService = soTietKiemService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isRole = await _userManager.IsInRoleAsync(currentUser, "User");
            Console.WriteLine(isRole);
            if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            return RedirectToAction("Dashboard", "User");
        }
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Dashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Thêm thống kê theo loại tiết kiệm
            var savingTypeStats = await _context.SoTietKiems
                .Where(s => s.UserId == currentUser.Id)
                .Include(s => s.LoaiSoTietKiem)
                .GroupBy(s => s.LoaiSoTietKiem.TenLoaiSo)
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Count(),
                    Amount = g.Sum(s => s.SoDuSoTietKiem)
                })
                .ToListAsync();

            ViewBag.SavingTypes = savingTypeStats.Select(s => s.Type).ToList();
            ViewBag.SavingTypeCounts = savingTypeStats.Select(s => s.Count).ToList();
            ViewBag.SavingTypeAmounts = savingTypeStats.Select(s => s.Amount).ToList();

            // Thêm thống kê tổng quan cho Polar Area Chart
            var totalStats = await _context.SoTietKiems
                .Where(s => s.UserId == currentUser.Id)
                .GroupBy(s => s.TrangThai)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            ViewBag.ActiveAccounts = totalStats.FirstOrDefault(s => s.Status)?.Count ?? 0;
            ViewBag.ClosedAccounts = totalStats.FirstOrDefault(s => !s.Status)?.Count ?? 0;

            // Lấy thống kê theo tháng trong năm hiện tại
            var currentYear = DateTime.Now.Year;
            var monthlyStats = await _context.SoTietKiems
                .Where(s => s.UserId == currentUser.Id && s.NgayMoSo.Year == currentYear)
                .GroupBy(s => s.NgayMoSo.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    OpenCount = g.Count(s => s.NgayMoSo.Month == g.Key),
                    CloseCount = g.Count(s => s.NgayDongSo != null && s.NgayDongSo.Value.Month == g.Key)
                })
                .ToListAsync();

            // Chuẩn bị dữ liệu cho biểu đồ theo tháng
            var months = Enumerable.Range(1, 12);
            var openData = months.Select(m => monthlyStats.FirstOrDefault(s => s.Month == m)?.OpenCount ?? 0).ToList();
            var closeData = months.Select(m => monthlyStats.FirstOrDefault(s => s.Month == m)?.CloseCount ?? 0).ToList();

            // Thống kê theo ngày trong tháng hiện tại
            var currentMonth = DateTime.Now.Month;
            var daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
            var dailyStats = await _context.SoTietKiems
                .Where(s => s.UserId == currentUser.Id &&
                            s.NgayMoSo.Year == currentYear &&
                            s.NgayMoSo.Month == currentMonth)
                .GroupBy(s => s.NgayMoSo.Day)
                .Select(g => new
                {
                    Day = g.Key,
                    OpenCount = g.Count(s => s.NgayMoSo.Day == g.Key),
                    CloseCount = g.Count(s => s.NgayDongSo != null && s.NgayDongSo.Value.Day == g.Key)
                })
                .ToListAsync();

            // Chuẩn bị dữ liệu cho biểu đồ theo ngày
            var days = Enumerable.Range(1, daysInMonth);
            var dailyOpenData = days.Select(d => dailyStats.FirstOrDefault(s => s.Day == d)?.OpenCount ?? 0).ToList();
            var dailyCloseData = days.Select(d => dailyStats.FirstOrDefault(s => s.Day == d)?.CloseCount ?? 0).ToList();

            ViewBag.MonthlyOpenData = openData;
            ViewBag.MonthlyCloseData = closeData;
            ViewBag.DailyOpenData = dailyOpenData;
            ViewBag.DailyCloseData = dailyCloseData;
            ViewBag.DaysInMonth = daysInMonth;

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Profile()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(currentUser);
        }
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ApplicationUser model)
        {
            _logger.LogInformation("EditProfile action called");
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                user.FullName = model.FullName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Profile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User) ?? throw new Exception("User not found");
                var result = await _userManager.ChangePasswordAsync(currentUser, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Dashboard");
                }
                //Add errors to ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult QuyDinhVaDieuKhoan()
        {
            ViewBag.QuyDinh = "Quy định và điều khoản";
            return View();
        }

    }
}
