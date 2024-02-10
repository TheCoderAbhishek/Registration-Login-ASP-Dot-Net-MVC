using Registration_Login_ASP_Dot_Net_MVC.Interfaces.EmailInterface;
using System.Net.Mail;
using System.Net;

namespace Registration_Login_ASP_Dot_Net_MVC.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly string? _smtpServer;
        private readonly int _port;
        private readonly string? _username;
        private readonly string? _password;

        public EmailService(IConfiguration configuration)
        {
            _smtpServer = configuration["EmailSettings:SmtpServer"];
            _port = configuration.GetValue<int>("EmailSettings:Port");
            _username = configuration["EmailSettings:Username"];
            _password = configuration["EmailSettings:Password"];
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            using (SmtpClient smtpClient = new(_smtpServer, _port))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_username, _password);
                smtpClient.EnableSsl = true;

                using (MailMessage mailMessage = new())
                {
                    mailMessage.From = new MailAddress(_username);
                    mailMessage.To.Add(recipientEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }

        public async Task SendOTPEmailAsync(string recipientEmail, string otp)
        {
            string subject = "OTP for Registration";
            string body = $"Your OTP for registration is: {otp}";
            await SendEmailAsync(recipientEmail, subject, body);
        }
    }
}
