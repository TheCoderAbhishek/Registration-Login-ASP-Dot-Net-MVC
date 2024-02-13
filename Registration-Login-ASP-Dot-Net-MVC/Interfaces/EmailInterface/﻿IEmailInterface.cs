namespace Registration_Login_ASP_Dot_Net_MVC.Interfaces.EmailInterface
{
    public interface ﻿IEmailInterface
    {
        Task SendEmailAsync(string recipientEmail, string subject, string body);
        Task SendOTPEmailAsync(string recipientEmail, string otp);
    }
}
