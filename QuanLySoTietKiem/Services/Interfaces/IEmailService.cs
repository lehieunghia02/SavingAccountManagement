using System.Threading.Tasks;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;

namespace QuanLySoTietKiem.Services.Interfaces
{
  public interface IEmailService
  {
    // Phương thức gửi email cơ bản
    Task SendEmailAsync(string toEmail, string subject, string body);

    // Thông báo khi mở sổ tiết kiệm mới
    Task SendAccountOpeningNotificationAsync(string toEmail, SoTietKiem soTietKiem);

    // Thông báo khi đóng sổ tiết kiệm
    Task SendAccountClosingNotificationAsync(string toEmail, SoTietKiem soTietKiem, decimal totalAmount);

    // Thông báo khi sổ tiết kiệm sắp đến hạn
    Task SendAccountMaturityNotificationAsync(string toEmail, SoTietKiem soTietKiem);

    // Thông báo khi có giao dịch mới (nạp tiền/rút tiền)
    Task SendTransactionNotificationAsync(string toEmail, GiaoDich giaoDich);
  }
}