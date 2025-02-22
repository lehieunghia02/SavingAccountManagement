using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using QuanLySoTietKiem.Configurations;
using QuanLySoTietKiem.Services.Interfaces;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly EmailSettings _emailSettings;
    public EmailService(IConfiguration configuration, IOptions<EmailSettings> emailSettings)
    {
        _configuration = configuration;
        _emailSettings = emailSettings.Value;
    }
    //Send email to user
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var mailConfig = _emailSettings;
            var message = new MailMessage();
            message.From = new MailAddress(
                mailConfig.FromEmail,
                mailConfig.FromName
            );
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var client = new SmtpClient())
            {
                client.Host = _emailSettings.SmtpServer;
                client.Port = _emailSettings.SmtpPort;
                client.EnableSsl = _emailSettings.EnableSsl;
                client.Credentials = new NetworkCredential(
                    _emailSettings.SmtpUsername,
                    _emailSettings.SmtpPassword
                );

                await client.SendMailAsync(message);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error sending email: {ex.Message}");
        }
    }
}