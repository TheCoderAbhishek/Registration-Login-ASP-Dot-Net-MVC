using Registration_Login_ASP_Dot_Net_MVC.Interfaces.EmailInterface;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.OtpAuthenticationInterface;

namespace Registration_Login_ASP_Dot_Net_MVC.Services.OtpService
{
    public class OTPService(IEmailService emailService) : IOTPService
    {
        private readonly IEmailService _emailService = emailService;

        // Generate OTP
        public string GenerateOTP()
        {
            Random random = new();
            return random.Next(100000, 999999).ToString();
        }

        // Send OTP via Email
        public async Task SendOTPByEmail(string email, string otp)
        {
            string subject = "Your OTP for registration";
            string body = $"Your OTP is: {otp}";

            await _emailService.SendEmailAsync(email, subject, body);
        }
    }
}
