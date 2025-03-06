using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Services.Interfaces;
using System.Threading.Tasks;

namespace QuanLySoTietKiem.Controllers
{
  [Authorize] // Yêu cầu đăng nhập
  public class NapTienController : Controller
  {
    private readonly ILogger<NapTienController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPayPalService _payPalService;
    private readonly IVNPayService _vnPayService;

    public NapTienController(
        ILogger<NapTienController> logger,
        UserManager<ApplicationUser> userManager,
        IPayPalService payPalService,
        IVNPayService vnPayService)
    {
      _logger = logger;
      _userManager = userManager;
      _payPalService = payPalService;
      _vnPayService = vnPayService;
    }

    // Hiển thị trang nạp tiền
    [HttpGet]
    public async Task<IActionResult> Index()
    {
      // Lấy thông tin người dùng hiện tại
      var user = await _userManager.GetUserAsync(User);
      if (user == null)
      {
        return RedirectToAction("Login", "Account");
      }

      // Tạo model với thông tin người dùng
      var model = new PaymentInformationModel
      {
        Name = user.FullName,
        OrderType = "Deposit", // Mặc định là nạp tiền
        CreatedDate = System.DateTime.Now
      };

      // Truyền thông tin số dư hiện tại vào ViewBag
      ViewBag.CurrentBalance = user.SoDuTaiKhoan;

      return View(model);
    }

    // Xử lý nạp tiền qua PayPal
    [HttpPost]
    public async Task<IActionResult> PayPal(PaymentInformationModel model)
    {
      if (!ModelState.IsValid)
      {
        return View("Index", model);
      }

      try
      {
        // Tạo URL thanh toán PayPal
        var paymentUrl = await _payPalService.CreatePaymentUrlAsync(model, HttpContext);

        // Chuyển hướng đến trang thanh toán PayPal
        return Redirect(paymentUrl);
      }
      catch (System.Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi tạo URL thanh toán PayPal");
        ModelState.AddModelError("", "Đã xảy ra lỗi khi kết nối với PayPal. Vui lòng thử lại sau.");
        return View("Index", model);
      }
    }

    // Xử lý nạp tiền qua VNPay
    [HttpPost]
    public IActionResult VNPay(PaymentInformationModel model)
    {
      if (!ModelState.IsValid)
      {
        return View("Index", model);
      }

      try
      {
        // Tạo URL thanh toán VNPay
        var paymentUrl = _vnPayService.CreatePaymentUrl(model, HttpContext);

        // Chuyển hướng đến trang thanh toán VNPay
        return Redirect(paymentUrl);
      }
      catch (System.Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi tạo URL thanh toán VNPay");
        ModelState.AddModelError("", "Đã xảy ra lỗi khi kết nối với VNPay. Vui lòng thử lại sau.");
        return View("Index", model);
      }
    }

    // Xử lý callback từ PayPal
    [HttpGet]
    public async Task<IActionResult> PayPalCallback(string paymentId, string PayerID)
    {
      if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(PayerID))
      {
        return RedirectToAction("PaymentFailed", "Payment", new { message = "Thanh toán đã bị hủy hoặc thất bại" });
      }

      try
      {
        // Xử lý kết quả thanh toán
        var result = await _payPalService.ExecutePaymentAsync(paymentId, PayerID, HttpContext);

        if (result.Status == "success")
        {
          // Sử dụng phương thức ToPaymentResponseModel có sẵn thay vì tạo đối tượng mới
          var paymentResponse = result.ToPaymentResponseModel();

          // Thanh toán thành công
          return RedirectToAction("PaymentSuccess", "Payment", paymentResponse);
        }
        else
        {
          // Thanh toán thất bại
          return RedirectToAction("PaymentFailed", "Payment", new { message = result.ErrorMessage });
        }
      }
      catch (System.Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi xử lý callback từ PayPal");
        return RedirectToAction("PaymentFailed", "Payment", new { message = "Đã xảy ra lỗi khi xử lý thanh toán" });
      }
    }

    // Xử lý hủy thanh toán PayPal
    [HttpGet]
    public IActionResult PayPalCancel()
    {
      return RedirectToAction("PaymentFailed", "Payment", new { message = "Thanh toán đã bị hủy" });
    }
  }
}