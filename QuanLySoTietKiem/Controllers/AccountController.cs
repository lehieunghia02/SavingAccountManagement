using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Services.Interfaces;
using LoginModel = QuanLySoTietKiem.Models.AccountModels.LoginModel.LoginModel;
using RegisterModel = QuanLySoTietKiem.Models.AccountModels.RegisterrModel.RegisterModel;

namespace QuanLySoTietKiem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger; 

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, 
            IAccountService  accountService, 
            IEmailService emailService, 
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountService = accountService;
            _emailService = emailService;
            _logger = logger; 
        }
        //Đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "User");
            }
            return View();
        }
        //Xử lý đăng nhập
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    ModelState.AddModelError("", "User does not exist.");
                    return View(model);
                }

                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError("", "Email chưa được xác thực, vui lòng kiểm tra inbox");
                    return View(model);
                }

                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                {
                    ModelState.AddModelError("", "Tài khoản đã bị khóa do nhiều lần đăng nhập sai.");
                    return View(model);
                }

                var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!isPasswordCorrect)
                {
                    ModelState.AddModelError("", "Sai mật khẩu.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Your account is locked due to multiple failed attempts.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }

            return RedirectToAction("Index", "User");
        }
        //Đăng ký tài khoản
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "User");
            }
            return View();
        }
        //Xử lý đăng ký tài khoản
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    Address = model.Address,
                    CCCD = model.CCCD,
                    SoDuTaiKhoan = model.SoDuTaiKhoan,
                    PhoneNumber = model.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                //assign role 
                if (result.Succeeded)
                {
                    Console.WriteLine("User created successfully");
                    await _userManager.AddToRoleAsync(user, "User");
                }
                //Login
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        //Xử lý logout 
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        //Quên mật khẩu
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        // Quên mật khẩu
        /*[HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.ForgotPassword(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Email đã được gửi đến bạn. Vui lòng kiểm tra email để đặt lại mật khẩu.";
                    return View("Login");
                }
                ModelState.AddModelError("", "Email không tồn tại trong hệ thống.");            
            }
            return View(model);
        }*/
        // [HttpGet]
        // public IActionResult ResetPassword(string email, string token)
        // {
        //     var model = new Res
        // }
    }

}
