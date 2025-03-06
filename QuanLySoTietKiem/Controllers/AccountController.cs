using System.Security.Claims;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models.AccountModels.UpdateAvatarModel;
using QuanLySoTietKiem.Services.Interfaces;
using ForgotPasswordModel = QuanLySoTietKiem.Models.AccountModels.ForgotPassword.ForgotPasswordModel;
using IAuthenticationService = QuanLySoTietKiem.Services.Interfaces.IAuthenticationService;
using LoginModel = QuanLySoTietKiem.Models.AccountModels.LoginModel.LoginModel;
using RegisterModel = QuanLySoTietKiem.Models.AccountModels.RegisterModel.RegisterModel;
using ResetPasswordModel = QuanLySoTietKiem.Models.AccountModels.ResetPasswordModel.ResetPasswordModel;

namespace QuanLySoTietKiem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthenticationService _authenticationService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IAccountService accountService,
            IEmailService emailService,
            ILogger<AccountController> logger,
            IAuthenticationService authenticationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountService = accountService;
            _emailService = emailService;
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            var isAuthenticated = User.Identity?.IsAuthenticated;

            if (isAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var (succeeded, message, returnUrl) = await _authenticationService.LoginAsync(
            model.UserName,
            model.Password);
            if (!succeeded)
            {
                ModelState.AddModelError("", message);
                return View(model);
            }
            return Redirect(returnUrl);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "User");
            }
            return View();
        }
        //Register account 
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
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
            //Assignment role 
            if (result.Succeeded)
            {
                //Assign role to user
                await _userManager.AddToRoleAsync(user, RoleConstants.User);
            }
            //Process login
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authenticationService.LogoutAsync(); 
            return RedirectToAction("Login", "Account");
        }
        
        //Đổi mật khẩu
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(model.Email))
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
        }
        
        //Cập nhật ảnh đại diện
        [HttpGet]
        [Authorize(Policy = PolicyConstants.RequireAdminOrUser)]
        public IActionResult UpdateAvatar()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Policy = PolicyConstants.RequireAdminOrUser)]
        public async Task<IActionResult> UpdateAvatar([FromForm] UpdateAvatarModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy tài khoản");
                    return View(model);
                }
                var avatarUrl = await _accountService.UploadAvatarAsync(userId, model.AvatarImage);
                if (avatarUrl == null)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật ảnh đại diện");
                    return View(model);
                }
                TempData["SuccessMessage"] = "Ảnh đại diện đã được cập nhật thành công";
                return RedirectToAction("Profile", "User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating avatar");
                ModelState.AddModelError("", "Lỗi khi cập nhật ảnh đại diện");
                return View(model);
            }
        }

        //Reset mật khẩu 
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }

            return View(new ResetPasswordModel { Token = token, Email = email });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); 
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description); 
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


    }
}
