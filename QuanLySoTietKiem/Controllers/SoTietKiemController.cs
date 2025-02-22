using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.RutTien;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Models.SavingsAccount;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Controllers
{
    public class SoTietKiemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SoTietKiemController> _logger;
        private readonly ISoTietKiemService _soTietKiemService;


        [ActivatorUtilitiesConstructor]
        public SoTietKiemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<SoTietKiemController> logger, ISoTietKiemService soTietKiemService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _soTietKiemService = soTietKiemService;
        }
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var soTietKiems = await _context.SoTietKiems.Include(s => s.LoaiSoTietKiem).Include(s => s.HinhThucDenHan)
                .Where(s => s.UserId == currentUser.Id)
                .ToListAsync();
            var model = soTietKiems.Select(stk => new SoTietKiemModel
            {
                MaSoTietKiem = stk.MaSoTietKiem,
                UserId = stk.UserId,
                SoTienGui = stk.SoTienGui,
                NgayMoSo = stk.NgayMoSo,
                MaHinhThucDenHan = stk.MaHinhThucDenHan,
                LaiSuatApDung = stk.LaiSuatApDung,
                NgayDongSo = stk.NgayDongSo ?? DateTime.Now.AddDays(stk.MaLoaiSo * 30),
                TrangThai = stk.TrangThai,
                Code = stk.Code,
                MaLoaiSo = stk.MaLoaiSo,
                SoDuSoTietKiem = stk.SoDuSoTietKiem,
                NgayDaoHan = stk.NgayDaoHan,
                TenLoaiSo = stk.LoaiSoTietKiem?.TenLoaiSo ?? "",  // Get TenLoaiSo from navigation property
                KyHan = stk.LoaiSoTietKiem?.KyHan ?? 0,  // Get KyHan from navigation property
                TenHinhThucDenHan = stk.HinhThucDenHan?.TenHinhThucDenHan ?? ""
            });
            return View(model);
        }
        public async Task<IActionResult> XuLyDaoHan(int maSoTietKiem)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var soTietKiem = await _context.SoTietKiems
                    .Include(s => s.LoaiSoTietKiem)
                    .FirstOrDefaultAsync(s => s.MaSoTietKiem == maSoTietKiem);
                if (soTietKiem == null)
                {
                    return NotFound();
                }
                // Tính tiền lãi
                var soNgayGui = (DateTime.Now - soTietKiem.NgayMoSo).Days;
                decimal tienLai = LaiSuatHelper.TinhTienLai(
                    soTietKiem.SoDuSoTietKiem,
                    soTietKiem.LaiSuatKyHan,
                    soNgayGui
                );
                // Xử lý đáo hạn
                await DaoHanHelper.XuLyDaoHan(soTietKiem, tienLai, _context);
                // Lưu thay đổi vào database
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return View("Error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> OpenSavingsAccount()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // Lấy số dư tài khoản của người dùng
            ViewBag.SoDuHienTai = currentUser?.SoDuTaiKhoan ?? 0;
            // log số dư tài khoản của người dùng
            _logger.LogInformation("SoDuHienTai: {SoDuHienTai}", currentUser?.SoDuTaiKhoan);
            // Fetch HinhThucDenHan options synchronously
            var hinhThucDenHanOptions = await _context.HinhThucDenHans.ToListAsync();
            _logger.LogInformation("HinhThucDenHanOptions: {HinhThucDenHanOptions}", hinhThucDenHanOptions.Select(ht => ht.TenHinhThucDenHan));
            ViewBag.HinhThucDenHanOptions = new SelectList(hinhThucDenHanOptions, "MaHinhThucDenHan", "TenHinhThucDenHan");
            ViewBag.CodeSTK = GenerateCode(currentUser?.Id ?? "");
            //Fetch LoaiSoTietKiem 
            var LoaiSoTietKiemOptions = await _context.LoaiSoTietKiems.ToListAsync();
            _logger.LogInformation("LoaiSoTietKiemOptions: {LoaiSoTietKiemOptions}", LoaiSoTietKiemOptions.Select(ls => ls.TenLoaiSo));
            ViewBag.LoaiSoTietKiemOptions = new SelectList(LoaiSoTietKiemOptions, "MaLoaiSo", "TenLoaiSo");
            return View();
        }
        //Xử lý mở tài khoản tiết kiệm
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> OpenSavingsAccount(SoTietKiemModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // Set required properties before ModelState validation
            model.UserId = currentUser.Id;
            model.NgayMoSo = DateTime.Now;
            model.TrangThai = true;
            model.Code = GenerateCode(currentUser.Id);
            model.SoDuSoTietKiem = model.SoTienGui;

            //Fetch KyHan from LoaiSoTietKieMModel table 
            var loaiSoTietKiem = await _context.LoaiSoTietKiems
            .FirstOrDefaultAsync(ls => ls.MaLoaiSo == model.MaLoaiSo);

            if (loaiSoTietKiem == null)
            {
                ModelState.AddModelError("MaLoaiSo", "Loại sổ tiết kiệm không tồn tại");
                await PopulateViewBagDropdowns();
                return View(model);
            }
            model.KyHan = loaiSoTietKiem.KyHan;
            model.LaiSuatKyHan = ((decimal)loaiSoTietKiem.LaiSuat) / 100; //chia cho 100
            model.LaiSuatApDung = ((decimal)loaiSoTietKiem.LaiSuat) / 100;

            _logger.LogInformation("LaiSuatKyHan: {LaiSuatKyHan}", model.LaiSuatKyHan);
            _logger.LogInformation("LaiSuatApDung: {LaiSuatApDung}", model.LaiSuatApDung);

            // Calculate NgayDaoHan 
            model.NgayDaoHan = model.NgayMoSo.AddMonths(loaiSoTietKiem.KyHan);
            _logger.LogInformation("NgayDaoHan: {NgayDaoHan}", model.NgayDaoHan);
            _logger.LogInformation("TenLoaiSo: {TenLoaiSo}", model.TenLoaiSo);


            // Clear ModelState and revalidate with the updated values
            ModelState.Clear();
            if (TryValidateModel(model))
            {
                if (currentUser.SoDuTaiKhoan < (double)model.SoTienGui)
                {
                    ModelState.AddModelError("SoTienGui", "Số dư tài khoản không đủ");
                    ViewBag.SoDuHienTai = currentUser.SoDuTaiKhoan;
                    await PopulateViewBagDropdowns();
                    return View(model);
                }
                var soTietKiem = new SoTietKiem
                {
                    UserId = currentUser.Id,
                    SoTienGui = model.SoTienGui,
                    NgayMoSo = model.NgayMoSo,
                    MaHinhThucDenHan = model.MaHinhThucDenHan,
                    MaLoaiSo = model.MaLoaiSo,
                    LaiSuatApDung = model.LaiSuatApDung,
                    LaiSuatKyHan = model.LaiSuatKyHan,
                    TrangThai = model.TrangThai,
                    Code = model.Code,
                    SoDuSoTietKiem = model.SoDuSoTietKiem,
                    NgayDaoHan = model.NgayDaoHan,
                    NgayDongSo = null
                };

                _logger.LogInformation("SoTietKiem: {@SoTietKiem}", soTietKiem);
                currentUser.SoDuTaiKhoan -= (double)model.SoTienGui;
                await _userManager.UpdateAsync(currentUser);

                await _context.SoTietKiems.AddAsync(soTietKiem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            _logger.LogError("ModelState is not valid: {@ModelStateErrors}",
                ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

            await PopulateViewBagDropdowns();
            ViewBag.SoDuHienTai = currentUser.SoDuTaiKhoan;
            ViewBag.CodeSTK = model.Code;
            return View(model);
        }
        //Hàm tạo code sổ tiết kiệm
        private string GenerateCode(string userId)
        {
            return "STK" + "-" + userId + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private async Task PopulateViewBagDropdowns()
        {
            var hinhThucDenHanOptions = await _context.HinhThucDenHans.ToListAsync();
            ViewBag.HinhThucDenHanOptions = new SelectList(hinhThucDenHanOptions, "MaHinhThucDenHan", "TenHinhThucDenHan");

            var LoaiSoTietKiemOptions = await _context.LoaiSoTietKiems.ToListAsync();
            ViewBag.LoaiSoTietKiemOptions = new SelectList(LoaiSoTietKiemOptions, "MaLoaiSo", "TenLoaiSo");
        }
        //Lấy thông tin chi tiết sổ tiết kiệm
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var soTietKiem = await _context.SoTietKiems
                .Include(s => s.LoaiSoTietKiem)
                .Include(s => s.HinhThucDenHan)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.MaSoTietKiem == id && s.UserId == currentUser.Id); // Ensure you check for UserId

            if (soTietKiem == null)
            {
                return NotFound(); // This will return a 404 if the code does not exist
            }
            // Tính lãi suất áp dụng
            decimal laiSuatApDung = LaiSuatHelper.TinhLaiSuatRutTien(
                soTietKiem.NgayMoSo,
                soTietKiem.NgayDaoHan,
                DateTime.Now,
                soTietKiem.LaiSuatKyHan
            );
            _logger.LogInformation("Laisuatkyhan: {Laisuatkyhan}", soTietKiem.LaiSuatKyHan);
            var soNgayGui = (DateTime.Now - soTietKiem.NgayMoSo).Days;
            decimal tienLai = LaiSuatHelper.TinhTienLai(
                soTietKiem.SoDuSoTietKiem,
                laiSuatApDung,
                soNgayGui
            );
            _logger.LogInformation("SoDuSoTietkiem: {SoDuSoTietkiem}", soTietKiem.SoDuSoTietKiem);
            _logger.LogInformation("LaiSuatApDung123: {LaiSuatApDung}", laiSuatApDung);
            _logger.LogInformation("NgayDongSo: {NgayDongSo}", soTietKiem.NgayDongSo);
            _logger.LogInformation("soNgayGui: {soNgayGui}", soNgayGui);
            _logger.LogInformation("TienLai: {TienLai}", tienLai);
            ViewBag.LaiSuatApDung = laiSuatApDung;
            ViewBag.TienLai = tienLai;
            ViewBag.TongTienNhanDuoc = soTietKiem.SoDuSoTietKiem + tienLai;


            var model = new SoTietKiemDetailModel
            {
                MaSoTietKiem = soTietKiem.MaSoTietKiem,
                Code = soTietKiem.Code ?? "",
                SoTienGui = soTietKiem.SoTienGui,
                SoDuSoTietKiem = soTietKiem.SoDuSoTietKiem,
                LaiSuatApDung = soTietKiem.LaiSuatApDung,
                LaiSuatKyHan = soTietKiem.LaiSuatKyHan,
                NgayMoSo = soTietKiem.NgayMoSo,
                NgayDongSo = soTietKiem.NgayDongSo,
                NgayDaoHan = soTietKiem.NgayDaoHan,
                TrangThai = soTietKiem.TrangThai,
                TenLoaiSo = soTietKiem.LoaiSoTietKiem?.TenLoaiSo ?? "",
                KyHan = soTietKiem.LoaiSoTietKiem?.KyHan ?? 0,
                TenHinhThucDenHan = soTietKiem.HinhThucDenHan?.TenHinhThucDenHan ?? "",
                SoTienThucHuong = soTietKiem.SoDuSoTietKiem + tienLai,
                TenKhachHang = soTietKiem.User?.FullName ?? "unknown"
            };
            return View(model);
        }
        //Load trang nạp tiền vào sổ tiết kiệm
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddMoney(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var soTietKiem = await _context.SoTietKiems.FirstOrDefaultAsync(m => m.MaSoTietKiem == id);
            if (!IsAddMoney(DateTime.Now, soTietKiem.NgayDaoHan))
            {
                TempData["Message"] = "Chưa tới ngày đáo hạn để nạp thêm tiền 😊";
                return RedirectToAction("Index");
            }
            ViewBag.CodeSTK = await _soTietKiemService.GetCodeSTK(currentUser.Id, id);
            ViewBag.SoDuHienTai = currentUser.SoDuTaiKhoan;
            if (soTietKiem == null)
            {
                return NotFound();
            }
            var model = new AddMoneyViewModel
            {
                MaSoTietKiem = id,
                SoDuHienTai = soTietKiem.SoDuSoTietKiem
            };
            return View(model);
        }
        //Hàm kiểm tra ngày đáo hạn
        private bool IsAddMoney(DateTime currentDate, DateTime ngayDaoHan)
        {
            if (currentDate.Date == ngayDaoHan.Date || currentDate.Date == ngayDaoHan.Date.AddDays(1))
            {
                return true;
            }
            return false;
        }
        //Xử lý nạp tiền vào sổ tiết kiệm
        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMoney(AddMoneyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var soTietKiem = await _context.SoTietKiems.FirstOrDefaultAsync(m => m.MaSoTietKiem == model.MaSoTietKiem);
            if (soTietKiem == null)
            {
                return NotFound();
            }

            //Kiểm tra số dư tài khoản của người dùng
            if (currentUser.SoDuTaiKhoan < (double)model.SoTienGui)
            {
                ModelState.AddModelError("SoTienGui", "Số dư tài khoản không đủ");
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //Cập nhật số dư sổ tiết kiệm
                soTietKiem.SoDuSoTietKiem += model.SoTienGui;

                //Trừ tiền từ tài khoản người dùng
                currentUser.SoDuTaiKhoan -= (double)model.SoTienGui;
                await _userManager.UpdateAsync(currentUser);

                //Tạo giao dịch mới 
                var giaoDich = new GiaoDich
                {
                    MaSoTietKiem = model.MaSoTietKiem,
                    MaLoaiGiaoDich = 2,
                    NgayGiaoDich = DateTime.Now,
                    SoTien = (double)model.SoTienGui,
                };
                await _context.GiaoDichs.AddAsync(giaoDich);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // TempData["SuccessMessage"] = $"Nạp tiền thành công. Mã giao dịch: {phieuGuiTien.MaGiaoDich}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi nạp tiền vào sổ tiết kiệm");
                ModelState.AddModelError("", "Có lỗi xảy ra khi nạp tiền. Vui lòng thử lại sau.");
                return View(model);
            }
        }

        private string GenerateTransactionCode(string userId)
        {
            return $"PGT-{userId.Substring(0, 4)}-{DateTime.Now:yyyyMMddHHmmss}";
        }

        //Xử lý rút tiền 
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> WithdrawMoney(int id)
        {
            //Lấy thông tin của người dùng
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Lấy thông tin sổ tiết kiệm
            var soTietKiem = await _context.SoTietKiems
            .Include(s => s.LoaiSoTietKiem)
            .FirstOrDefaultAsync(s => s.MaSoTietKiem == id);

            if (soTietKiem == null)
                return NotFound();

            var model = new WithdrawMoneyViewModel
            {
                MaSoTietKiem = soTietKiem.MaSoTietKiem,
                SoDuHienTai = soTietKiem.SoDuSoTietKiem,
                NgayMoSo = soTietKiem.NgayMoSo,
                NgayDaoHan = soTietKiem.NgayDaoHan,
                LaiSuatKyHan = soTietKiem.LaiSuatKyHan,
                Code = soTietKiem.Code,
                TrangThai = soTietKiem.TrangThai
            };

            // Tính lãi suất áp dụng cho sổ tiết kiệm
            decimal laiSuatApDung = LaiSuatHelper.TinhLaiSuatRutTien(
                soTietKiem.NgayMoSo,
                soTietKiem.NgayDaoHan,
                DateTime.Now,
                soTietKiem.LaiSuatKyHan
            );
            _logger.LogInformation("LaiSuatApDung: {LaiSuatApDung}", laiSuatApDung);
            // Tính tiền lãi
            decimal tienLai = LaiSuatHelper.TinhTienLai(
                soTietKiem.SoDuSoTietKiem,
                laiSuatApDung,
                (DateTime.Now - soTietKiem.NgayMoSo).Days
            );
            // Trường hợp rút đúng ngày đáo hạn
            if (DateTime.Now.Date == soTietKiem.NgayDaoHan.Date)
            {
                // Xử lý theo hình thức đáo hạn
                await DaoHanHelper.XuLyDaoHan(soTietKiem, tienLai, _context);
            }
            else
            {
                // Xử lý rút tiền bình thường
                soTietKiem.SoDuSoTietKiem -= model.SoTienRut;
            }

            ViewBag.LaiSuatApDung = laiSuatApDung;
            ViewBag.TienLai = tienLai;
            ViewBag.TongTienNhanDuoc = soTietKiem.SoDuSoTietKiem + tienLai;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> WithdrawMoney(WithdrawMoneyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var soTietKiem = await _context.SoTietKiems
                .Include(s => s.LoaiSoTietKiem)
                .FirstOrDefaultAsync(s => s.MaSoTietKiem == model.MaSoTietKiem);

            if (soTietKiem == null)
            {
                return NotFound();
            }

            // Tính lãi suất áp dụng
            decimal laiSuatApDung = LaiSuatHelper.TinhLaiSuatRutTien(
                soTietKiem.NgayMoSo,
                soTietKiem.NgayDaoHan,
                DateTime.Now,
                soTietKiem.LaiSuatKyHan
            );

            // Tính tiền lãi
            decimal tienLai = LaiSuatHelper.TinhTienLai(
                soTietKiem.SoDuSoTietKiem,
                laiSuatApDung,
                (DateTime.Now - soTietKiem.NgayMoSo).Days
            );

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra số tiền rút có hợp lệ
                if (model.SoTienRut > soTietKiem.SoDuSoTietKiem)
                {
                    ModelState.AddModelError("SoTienRut", "Số tiền rút không được lớn hơn số dư");
                    return View(model);
                }

                // Cập nhật số dư sổ tiết kiệm
                soTietKiem.SoDuSoTietKiem -= model.SoTienRut;

                // Cộng tiền vào tài khoản người dùng
                currentUser.SoDuTaiKhoan += (double)(model.SoTienRut + tienLai);
                await _userManager.UpdateAsync(currentUser);

                // Tạo giao dịch mới
                var giaoDich = new GiaoDich
                {
                    MaSoTietKiem = model.MaSoTietKiem,
                    MaLoaiGiaoDich = 1, // 1 là mã loại giao dịch rút tiền
                    NgayGiaoDich = DateTime.Now,
                    SoTien = (double)model.SoTienRut
                };

                // Nếu rút hết, đóng sổ
                if (soTietKiem.SoDuSoTietKiem == 0)
                {
                    soTietKiem.TrangThai = false;
                    soTietKiem.NgayDongSo = DateTime.Now;
                }

                await _context.GiaoDichs.AddAsync(giaoDich);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi rút tiền từ sổ tiết kiệm");
                ModelState.AddModelError("", "Có lỗi xảy ra khi rút tiền. Vui lòng thử lại sau.");
                return View(model);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteSavingsAccount(int id)
        {
            /*var soTietKiem = await _context.SoTietKiems.FindAsync(id);
            if (soTietKiem == null)
                return NotFound();
            if (soTietKiem.TrangThai)
            {
                ModelState.AddModelError("", "Sổ tiết kiệm đang hoạt động, không thể xóa");
                return View("Details", soTietKiem);
            }

            _context.SoTietKiems.Remove(soTietKiem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));*/
            return null; 
        }
    }
}
