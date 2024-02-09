using Microsoft.EntityFrameworkCore;
using Registration_Login_ASP_Dot_Net_MVC.Data;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.AccountInterfaces;
using Registration_Login_ASP_Dot_Net_MVC.Models.AccountModel;
using System;
using System.Security.Cryptography;
using System.Text;
using static Registration_Login_ASP_Dot_Net_MVC.Models.AccountModel.Users;

namespace Registration_Login_ASP_Dot_Net_MVC.Services.AccountService
{
    public class AccountService : IAccountInterface
    {
        private readonly AccountDbContext _accountDbContext;

        public AccountService(AccountDbContext accountDbContext)
        {
            _accountDbContext = accountDbContext;
        }

        public async Task<bool> IsEmailAlreadyRegistered(string email)
        {
            return await _accountDbContext.Users.AnyAsync(u => u.Email == email);
        }

        public async Task RegisterUser(RegisterViewModel registerViewModel)
        {
            var hashedPassword = HashPassword(registerViewModel.Password);

            var user = new User
            {
                Username = registerViewModel.Username,
                Email = registerViewModel.Email,
                PasswordHash = hashedPassword
            };

            _accountDbContext.Users.Add(user);
            await _accountDbContext.SaveChangesAsync();
        }

        public async Task<bool> Authenticate(string email, string password)
        {
            var user = await _accountDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                // Hash the provided password and compare it with the stored password hash
                var hashedPassword = HashPassword(password);
                return hashedPassword == user.PasswordHash;
            }

            return false;
        }

        // Helper method to hash password
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
