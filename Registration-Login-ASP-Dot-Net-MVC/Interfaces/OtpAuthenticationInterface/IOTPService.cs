namespace Registration_Login_ASP_Dot_Net_MVC.Interfaces.OtpAuthenticationInterface
{
    public interface IOTPService
    {
        string GenerateOTP();
        Task SendOTPByEmail(string email, string otp);
    }
}
