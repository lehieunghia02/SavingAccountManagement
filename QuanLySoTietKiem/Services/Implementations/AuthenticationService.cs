
using Microsoft.AspNetCore.Identity;
using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Entity;
using IAuthenticationService = QuanLySoTietKiem.Services.Interfaces.IAuthenticationService;

namespace QuanLySoTietKiem.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<(bool succeeded, string message, string returnUrl)> LoginAsync(
        string userName,
        string password,
        bool isPersistent = false)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return (false, MessageConstants.Login.AccountNotFound, string.Empty);
                }

                if (!user.EmailConfirmed)
                {
                    return (false, MessageConstants.Login.EmailNotConfirmed, string.Empty);
                }

                // Kiểm tra tài khoản có bị khóa
                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                {
                    return (false, MessageConstants.Login.AccountLocked, string.Empty);
                }

                // Kiểm tra mật khẩu
                var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
                if (!isPasswordCorrect)
                {
                    return (false, MessageConstants.Login.InvalidPassword, string.Empty);
                }

                // Thực hiện đăng nhập
                var result = await _signInManager.PasswordSignInAsync(
                    userName,
                    password,
                    isPersistent,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Kiểm tra role và trả về URL tương ứng
                    var roles = await _userManager.GetRolesAsync(user);
                    var returnUrl = roles.Contains(RoleConstants.Admin)
                        ? "/Admin"
                        : "/User";
                    return (true, MessageConstants.Login.LoginSuccess, returnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {UserName} account locked out", userName);
                    return (false, "Tài khoản đã bị khóa do nhiều lần đăng nhập sai.", string.Empty);
                }

                return (false, MessageConstants.Login.LoginFailed, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {UserName}", userName);
                return (false, MessageConstants.Login.ErrorDuringPassword, string.Empty);
            }
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
        }
    }
}
