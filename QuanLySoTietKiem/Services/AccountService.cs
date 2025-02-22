using Microsoft.AspNetCore.Identity;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Models.AccountModels.ForgotPassword;
using QuanLySoTietKiem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace QuanLySoTietKiem.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IEmailService _emailService; 
        // private readonly IUrlHelper _urlHelper;
        // private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
                // _urlHelper = urlHelper;
                // _httpContextAccessor = httpContextAccessor;
        }


        public async Task<bool> ForgotPassword(ForgotPasswordModel model)
        {
            // var user = await _userManager.FindByEmailAsync(model.Email);
            // if (user == null)
            // {
            //     return false;
            // }

            // var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // var callbackUrl = _urlHelper.Action(
            //     "ResetPassword",
            //     "Account",
            //     new { email = model.Email, token = token },
            //     _httpContextAccessor.HttpContext.Request.Scheme);

            // var message = $@"
            // <h3>Đặt lại mật khẩu</h3>
            // <p>Vui lòng nhấp vào <a href='{callbackUrl}'>đây</a> để đặt lại mật khẩu của bạn.</p>
            // <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>";

            // await _emailService.SendEmailAsync(
            //     model.Email,
            //     "Đặt lại mật khẩu",
            //     message);

            // return true;
            return true;
        }


    }
}
