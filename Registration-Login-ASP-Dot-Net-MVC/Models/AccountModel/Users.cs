namespace Registration_Login_ASP_Dot_Net_MVC.Models.AccountModel
{
    public class Users
    {
        public class User
        {
            public int UserId { get; set; }
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? PasswordHash { get; set; }
        }
    }
}
