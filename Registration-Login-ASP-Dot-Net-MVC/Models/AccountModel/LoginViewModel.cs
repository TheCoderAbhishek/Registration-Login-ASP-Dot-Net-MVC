using System.ComponentModel.DataAnnotations;

namespace Registration_Login_ASP_Dot_Net_MVC.Models.AccountModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
