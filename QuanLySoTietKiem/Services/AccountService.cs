using Microsoft.AspNetCore.Identity;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Models.AccountModels.ForgotPassword;
using QuanLySoTietKiem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using QuanLySoTietKiem.Entity;

namespace QuanLySoTietKiem.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IEmailService _emailService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<AccountService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelper _urlHelper;

        public AccountService(
            UserManager<ApplicationUser> userManager, 
            IEmailService emailService, 
            ICloudinaryService cloudinaryService,
            ILogger<AccountService> logger,
            IHttpContextAccessor httpContextAccessor,
            IUrlHelper urlHelper)
        {
            _userManager = userManager;
            _emailService = emailService;
            _urlHelper = urlHelper;
            _httpContextAccessor = httpContextAccessor;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }


        public async Task<bool> ForgotPassword(ForgotPasswordModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                throw new ArgumentException("Email address cannot be empty", nameof(model.Email));
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return false;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = _urlHelper.Action(
                "ResetPassword",
                "Account",
                new { email = model.Email, token = token },
                _httpContextAccessor.HttpContext.Request.Scheme);

            var message = $@"
             <h3>Đặt lại mật khẩu</h3>
             <p>Vui lòng nhấp vào <a href='{callbackUrl}'>đây</a> để đặt lại mật khẩu của bạn.</p>
             <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>";

            await _emailService.SendEmailAsync(
                model.Email,
                "Đặt lại mật khẩu",
                message);

            return true;
        }
        //forgot password 


        public async Task<string> UploadAvatarAsync(string userId, IFormFile avatarImage)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                var imageUrl = await _cloudinaryService.UploadImageAsync(avatarImage);

                user.AvatarUrl = imageUrl;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to update user avatar");
                }
                _logger.LogInformation("Avatar uploaded successfully");
                return imageUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar");
                throw;
            }

        }
    }
}
