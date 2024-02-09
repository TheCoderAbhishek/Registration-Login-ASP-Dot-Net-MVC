using Registration_Login_ASP_Dot_Net_MVC.Models.AccountModel;
using System.Threading.Tasks;

namespace Registration_Login_ASP_Dot_Net_MVC.Interfaces.AccountInterfaces
{
    public interface IAccountInterface
    {
        Task<bool> IsEmailAlreadyRegistered(string email);
        Task RegisterUser(RegisterViewModel model);
        Task<bool> Authenticate(string email, string password);
    }
}
