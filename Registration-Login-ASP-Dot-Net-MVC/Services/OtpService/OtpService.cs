using Registration_Login_ASP_Dot_Net_MVC.Interfaces.EmailInterface;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.OtpInterface;

namespace Registration_Login_ASP_Dot_Net_MVC.Services.OtpService
{
    public class OtpService(IEmailInterface emailInterface) : IOtpInterface
    {
        private readonly IEmailInterface _emailInterface = emailInterface;

        public string GenerateOTP()
        {
            Random random = new();
            return random.Next(100000, 999999).ToString();
        }

        public async Task SendOTPByEmail(string email, string otp)
        {
            string subject = "Your OTP for registration";
            string body = $"Your OTP is: {otp}";

            await _emailInterface.SendEmailAsync(email, subject, body);
        }
    }
}
