using System.ComponentModel.DataAnnotations;

namespace Registration_Login_ASP_Dot_Net_MVC.Models.AccountModel
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string? Username { get; set; }

        [Required]
        [RegularExpression(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$", ErrorMessage = "Invalid email address format.")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
