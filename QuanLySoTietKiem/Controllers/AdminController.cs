using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Models.RoleViewModels;

namespace QuanLySoTietKiem.Controllers
{
    [Authorize(Roles = RoleConstants.Admin)]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        [ActivatorUtilitiesConstructor]
        public AdminController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<AdminController> logger,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("");
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var roles = await _userManager.GetRolesAsync(currentUser);
            if (!roles.Contains(RoleConstants.Admin))
            {
                return RedirectToAction("Login", "Account");
            }

            if (roles.Contains(RoleConstants.User))
            {
                return RedirectToAction("Index", "User");
            }

            return RedirectToAction("Dashboard", "Admin");

        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var (deposits, withdrawals) = await GetTransactionDataByMonth();
            ViewBag.MonthlyDeposits = deposits;
            ViewBag.MonthlyWithdrawals = withdrawals;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Report()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            return View();
        }
        // User Management  
        [HttpGet]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> UserManagement()
        {
            // Redirect to UserList action
            return RedirectToAction(nameof(UserList));
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> UserList()
        {
            var users = await _userManager.GetUsersInRoleAsync("User");
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpGet]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> ReportByDay()
        {
            // Get today's date at midnight
            var today = DateTime.Today;

            // Get all transactions for today
            var report = await GenerateDailyReport(today);
            return View(report);
        }
        [HttpPost]
        public async Task<IActionResult> ReportByDay(DateTime date)
        {
            var report = await GenerateDailyReport(date);
            return View(report);
        }
        private async Task<BaoCaoNgay> GenerateDailyReport(DateTime date)
        {
            try
            {
                // Get transactions for the specified date
                var transactions = await _context.GiaoDichs
                    .Include(g => g.SoTietKiem)
                    .ThenInclude(s => s.LoaiSoTietKiem)
                    .Where(g => g.NgayGiaoDich.Date == date.Date)
                    .ToListAsync();

                // Calculate totals
                decimal tongTienGui = transactions
                    .Where(t => t.MaLoaiGiaoDich == 2) // Mã 2 là giao dịch gửi tiền
                    .Sum(t => (decimal)t.SoTien);

                decimal tongTienRut = transactions
                    .Where(t => t.MaLoaiGiaoDich == 1) // Mã 1 là giao dịch rút tiền  
                    .Sum(t => (decimal)t.SoTien);

                // Tạo báo cáo mới
                var report = new BaoCaoNgay
                {
                    Ngay = date,
                    TongTienGui = tongTienGui,
                    TongTienRut = tongTienRut,
                    NgayTaoBaoCao = DateTime.Now,
                    MaLoaiSo = 1 // Mặc định là loại sổ 1, có thể điều chỉnh theo yêu cầu
                };

                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo báo cáo ngày: {Message}", ex.Message);
                // Trả về báo cáo trống nếu có lỗi
                return new BaoCaoNgay
                {
                    Ngay = date,
                    TongTienGui = 0,
                    TongTienRut = 0,
                    NgayTaoBaoCao = DateTime.Now,
                    MaLoaiSo = 1
                };
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportToPdf(DateTime date)
        {
            var report = await GenerateDailyReport(date);

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Add title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var title = new Paragraph($"Báo Cáo Ngày {date:dd/MM/yyyy}", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);
                document.Add(new Paragraph("\n"));

                // Add content
                var contentFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                document.Add(new Paragraph($"Tong tien gui: {report.TongTienGui:N0} VNĐ", contentFont));
                document.Add(new Paragraph($"Tong tien rut: {report.TongTienRut:N0} VNĐ", contentFont));
                document.Add(new Paragraph($"Chech lech: {(report.TongTienGui - report.TongTienRut):N0} VNĐ", contentFont));
                document.Add(new Paragraph($"Thoi diem tao bao cao: {report.NgayTaoBaoCao:dd/MM/yyyy HH:mm:ss}", contentFont));

                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", $"BaoCaoNgay_{date:ddMMyyyy}.pdf");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(DateTime date)
        {
            var report = await GenerateDailyReport(date);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Báo Cáo Ngày");

                // Add title
                worksheet.Cells["A1:D1"].Merge = true;
                worksheet.Cells["A1"].Value = $"Báo Cáo Ngày {date:dd/MM/yyyy}";
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Add content
                worksheet.Cells["A3"].Value = "Tổng tiền gửi:";
                worksheet.Cells["B3"].Value = report.TongTienGui;
                worksheet.Cells["B3"].Style.Numberformat.Format = "#,##0";

                worksheet.Cells["A4"].Value = "Tổng tiền rút:";
                worksheet.Cells["B4"].Value = report.TongTienRut;
                worksheet.Cells["B4"].Style.Numberformat.Format = "#,##0";

                worksheet.Cells["A5"].Value = "Chênh lệch:";
                worksheet.Cells["B5"].Value = report.TongTienGui - report.TongTienRut;
                worksheet.Cells["B5"].Style.Numberformat.Format = "#,##0";

                worksheet.Cells["A6"].Value = "Thời điểm tạo báo cáo:";
                worksheet.Cells["B6"].Value = report.NgayTaoBaoCao;
                worksheet.Cells["B6"].Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";

                worksheet.Columns.AutoFit();

                return File(package.GetAsByteArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"BaoCaoNgay_{date:ddMMyyyy}.xlsx");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReportByMonth()
        {
            var today = DateTime.Today;
            var report = await GenerateMonthlyReport(today.Month, today.Year);
            return View(report);

        }
        [HttpPost]
        public async Task<IActionResult> ReportByMonth(int month, int year)
        {
            var report = await GenerateMonthlyReport(month, year);
            return View(report);
        }

        private async Task<BaoCaoThang> GenerateMonthlyReport(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var transactions = await _context.GiaoDichs
            .Include(g => g.SoTietKiem)
            .ThenInclude(s => s.LoaiSoTietKiem)
            .Where(g => g.NgayGiaoDich >= startDate && g.NgayGiaoDich <= endDate)
            .ToListAsync();

            // Calculate totals
            decimal tongTienGui = transactions
                .Where(t => t.MaLoaiGiaoDich == 2) // Mã 2 là giao dịch gửi tiền
                .Sum(t => (decimal)t.SoTien);

            decimal tongTienRut = transactions
            .Where(t => t.MaLoaiGiaoDich == 1) // Mã 1 là giao dịch rút tiền
            .Sum(t => (decimal)t.SoTien);

            var soLuongMo = await _context.SoTietKiems.CountAsync(s => s.NgayDongSo.HasValue && s.NgayDongSo.Value.Month == month && s.NgayDongSo.Value.Year == year);

            var soLuongDong = await _context.SoTietKiems.CountAsync(s => s.NgayDongSo.HasValue && s.NgayDongSo.Value.Month == month && s.NgayDongSo.Value.Year == year);
            var report = new BaoCaoThang
            {
                Thang = month,
                Nam = year,
                TongTienGui = tongTienGui,
                TongTienRut = tongTienRut,
                ChenhLech = tongTienGui - tongTienRut,
                SoLuongMo = 0, // Số lượng sổ mở
                SoLuongDong = 0, // Số lượng sổ đóng
                NgayTaoBaoCao = DateTime.Now
            };
            return report;
        }

        [HttpPost]
        public async Task<IActionResult> ExportMonthlyReportToPdf(int month, int year)
        {
            var report = await GenerateMonthlyReport(month, year);

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Add title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var title = new Paragraph($"Báo Cáo Tháng {month}/{year}", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);
                document.Add(new Paragraph("\n"));

                // Add content
                var contentFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                document.Add(new Paragraph($"Tổng tiền gửi: {report.TongTienGui:N0} VNĐ", contentFont));
                document.Add(new Paragraph($"Tổng tiền rút: {report.TongTienRut:N0} VNĐ", contentFont));
                document.Add(new Paragraph($"Chênh lệch: {report.ChenhLech:N0} VNĐ", contentFont));
                document.Add(new Paragraph($"Số lượng sổ mở mới: {report.SoLuongMo}", contentFont));
                document.Add(new Paragraph($"Số lượng sổ đóng: {report.SoLuongDong}", contentFont));
                document.Add(new Paragraph($"Thời điểm tạo báo cáo: {report.NgayTaoBaoCao:dd/MM/yyyy HH:mm:ss}", contentFont));

                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", $"BaoCaoThang_{month}_{year}.pdf");
            }
        }
        [HttpGet]

        public IActionResult ThayDoiQuyDinh()
        {
            return View();
        }

        private async Task<(List<decimal> deposits, List<decimal> withdrawals)> GetTransactionDataByMonth()
        {
            var currentYear = DateTime.Now.Year;
            var deposits = new List<decimal>();
            var withdrawals = new List<decimal>();
            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(currentYear, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                /*Tính tổng tiền gửi trong tháng*/
                /*var monthlyDeposits = await _context.GiaoDichs
                    .Where(g => g.NgayGiaoDich.Month == month
                            && g.NgayGiaoDich.Year == currentYear
                            && g.MaLoaiGiaoDich == 2) // 2 là mã giao dịch gửi tiền
                    .Select(g => g.SoTien)
                    .DefaultIfEmpty(0)
                    .SumAsync()*/
                ;

                // Tính tổng tiền rút trong tháng
                /*var monthlyWithdrawals = await _context.GiaoDichs
                    .Where(g => g.NgayGiaoDich.Month == month
                            && g.NgayGiaoDich.Year == currentYear
                            && g.MaLoaiGiaoDich == 1) // 1 là mã giao dịch rút tiền
                    .Select(g => g.SoTien)
                    .DefaultIfEmpty(0)
                    .SumAsync();*/

                //deposits.Add(Convert.ToDecimal(monthlyDeposits));
                //withdrawals.Add(Convert.ToDecimal(monthlyWithdrawals));
            }

            return (deposits, withdrawals);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,PhoneNumber,FullName,Address,CCCD")] ApplicationUser model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing user from the database
                    var user = await _userManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Update user properties
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.FullName = model.FullName;
                    user.Address = model.Address;
                    user.CCCD = model.CCCD;

                    // Save changes
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User {UserId} updated successfully by admin", id);
                        return RedirectToAction(nameof(UserList));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error occurred while updating user {UserId}", id);
                    if (!await UserExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(model);
        }

        // GET: Admin/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Admin/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                // Check if user has any related data before deletion
                var hasTransactions = await _context.GiaoDichs.AnyAsync(g => g.UserId == id);
                var hasSavingsAccounts = await _context.SoTietKiems.AnyAsync(s => s.UserId == id);

                if (hasTransactions || hasSavingsAccounts)
                {
                    TempData["ErrorMessage"] = "Không thể xóa người dùng này vì họ có giao dịch hoặc sổ tiết kiệm liên quan.";
                    return RedirectToAction(nameof(UserList));
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {UserId} deleted successfully by admin", id);
                    TempData["SuccessMessage"] = "Xóa người dùng thành công.";
                    return RedirectToAction(nameof(UserList));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa người dùng. Vui lòng thử lại sau.";
                return RedirectToAction(nameof(UserList));
            }
        }

        private async Task<bool> UserExists(string id)
        {
            return await _userManager.FindByIdAsync(id) != null;
        }

        // GET: Admin/ManageRoles
        [HttpGet]
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(UserList));
            }

            // Lấy tất cả roles trong hệ thống
            var roles = _roleManager.Roles.ToList();
            // Lấy roles hiện tại của user
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new ManageUserRolesViewModel
            {
                UserId = userId,
                UserName = user.UserName,
                Roles = roles.Select(r => new RoleViewModel
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    IsSelected = userRoles.Contains(r.Name)
                }).ToList()
            };

            return View(model);
        }

        // POST: Admin/ManageRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(ManageUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(UserList));
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            try
            {
                foreach (var role in model.Roles)
                {
                    // Nếu role được chọn và user chưa có role đó
                    if (role.IsSelected && !userRoles.Contains(role.RoleName))
                    {
                        await _userManager.AddToRoleAsync(user, role.RoleName);
                    }
                    // Nếu role không được chọn và user đang có role đó
                    else if (!role.IsSelected && userRoles.Contains(role.RoleName))
                    {
                        // Kiểm tra nếu đây là Admin role và user là admin cuối cùng
                        if (role.RoleName == RoleConstants.Admin)
                        {
                            var adminUsers = await _userManager.GetUsersInRoleAsync(RoleConstants.Admin);
                            if (adminUsers.Count == 1 && adminUsers.First().Id == user.Id)
                            {
                                TempData["ErrorMessage"] = "Không thể xóa quyền Admin của người dùng cuối cùng.";
                                return RedirectToAction(nameof(ManageRoles), new { userId = model.UserId });
                            }
                        }
                        await _userManager.RemoveFromRoleAsync(user, role.RoleName);
                    }
                }

                TempData["SuccessMessage"] = "Cập nhật quyền thành công.";
                return RedirectToAction(nameof(UserList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật quyền cho user {UserId}: {Message}", model.UserId, ex.Message);
                TempData["ErrorMessage"] = $"Lỗi khi cập nhật quyền: {ex.Message}";
                return RedirectToAction(nameof(ManageRoles), new { userId = model.UserId });
            }
        }

        // GET: Admin/RoleList
        [HttpGet]
        public IActionResult RoleList()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        // GET: Admin/CreateRole
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        // POST: Admin/CreateRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole([Bind("Name")] IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role.Name);
                if (roleExists)
                {
                    TempData["ErrorMessage"] = "Quyền này đã tồn tại.";
                    return View(role);
                }

                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Tạo quyền mới thành công.";
                    return RedirectToAction(nameof(RoleList));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(role);
        }

        // GET: Admin/EditRole/5
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Admin/EditRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(string id, [Bind("Id,Name")] IdentityRole role)
        {
            if (id != role.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRole = await _roleManager.FindByIdAsync(id);
                    if (existingRole == null)
                    {
                        return NotFound();
                    }

                    existingRole.Name = role.Name;
                    var result = await _roleManager.UpdateAsync(existingRole);
                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Cập nhật quyền thành công.";
                        return RedirectToAction(nameof(RoleList));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await RoleExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(role);
        }

        // GET: Admin/DeleteRole/5
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Admin/DeleteRole/5
        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoleConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            // Kiểm tra xem có phải role Admin không
            if (role.Name == RoleConstants.Admin)
            {
                TempData["ErrorMessage"] = "Không thể xóa quyền Admin.";
                return RedirectToAction(nameof(RoleList));
            }

            try
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Xóa quyền thành công.";
                    return RedirectToAction(nameof(RoleList));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa quyền {RoleId}: {Message}", id, ex.Message);
                TempData["ErrorMessage"] = $"Lỗi khi xóa quyền: {ex.Message}";
            }

            return View(role);
        }

        private async Task<bool> RoleExists(string id)
        {
            return await _roleManager.FindByIdAsync(id) != null;
        }
    }
}
