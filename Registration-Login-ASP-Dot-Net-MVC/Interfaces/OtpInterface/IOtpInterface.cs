namespace Registration_Login_ASP_Dot_Net_MVC.Interfaces.OtpInterface
{
    public interface IOtpInterface
    {
        string GenerateOTP();
        Task SendOTPByEmail(string email, string otp);
    }
}
