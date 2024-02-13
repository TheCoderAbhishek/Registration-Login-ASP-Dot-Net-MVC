using Registration_Login_ASP_Dot_Net_MVC.Interfaces.EmailInterface;
using System.Net.Mail;
using System.Net;

namespace Registration_Login_ASP_Dot_Net_MVC.Services.EmailService
{
    public class EmailService(IConfiguration configuration) : IEmailInterface
    {
        private readonly string? _smtpServer = configuration["EmailSettings:SmtpServer"];
        private readonly int _port = configuration.GetValue<int>("EmailSettings:Port");
        private readonly string? _username = configuration["EmailSettings:Username"];
        private readonly string? _password = configuration["EmailSettings:Password"];

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            if (_username != null)
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
            else
            {
                throw new InvalidOperationException("Email username is null.");
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
