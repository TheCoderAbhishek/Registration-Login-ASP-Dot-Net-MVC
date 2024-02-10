namespace Registration_Login_ASP_Dot_Net_MVC.Interfaces.EmailInterface
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipientEmail, string subject, string body);
        Task SendOTPEmailAsync(string recipientEmail, string otp);
    }
}
