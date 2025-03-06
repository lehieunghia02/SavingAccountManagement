using QuanLySoTietKiem.Models.AccountModels.ChangePasswordModel;
using QuanLySoTietKiem.Models.AccountModels.ForgotPassword;



namespace QuanLySoTietKiem.Services.Interfaces
{
    public interface IAccountService
    {
        Task<string> UploadAvatarAsync(string userId, IFormFile avatarImage);
        Task<bool> ForgotPassword(ForgotPasswordModel model); 

    }
}
