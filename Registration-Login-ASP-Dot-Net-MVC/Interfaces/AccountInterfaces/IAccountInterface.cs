using Registration_Login_ASP_Dot_Net_MVC.Models.AccountModel;

namespace Registration_Login_ASP_Dot_Net_MVC.Interfaces.AccountInterfaces
{
    public interface IAccountInterface
    {
        Task<bool> IsEmailAlreadyRegistered(string? email);
        Task RegisterUser(RegisterViewModel registerViewModel);
        Task<bool> Authenticate(string? email, string? password);
    }
}
