using Microsoft.EntityFrameworkCore;
using Registration_Login_ASP_Dot_Net_MVC.Data;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.AccountInterfaces;
using Registration_Login_ASP_Dot_Net_MVC.Models.AccountModel;
using System.Security.Cryptography;
using System.Text;
using static Registration_Login_ASP_Dot_Net_MVC.Models.AccountModel.Users;

namespace Registration_Login_ASP_Dot_Net_MVC.Services.AccountService
{
    public class AccountService(AccountDbContext accountDbContext, ILogger<AccountService> logger) : IAccountInterface
    {
        private readonly AccountDbContext _accountDbContext = accountDbContext;
        private readonly ILogger<AccountService> _logger = logger;

        public async Task<bool> IsEmailAlreadyRegistered(string? email)
        {
            try
            {
                var isRegistered = await _accountDbContext.Users.AnyAsync(u => u.Email == email);

                if (isRegistered)
                    _logger.LogInformation($"Email '{email}' is already registered.");
                else
                    _logger.LogInformation($"Email '{email}' is not registered.");

                return isRegistered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking email registration.");
                throw;
            }
        }

        public async Task RegisterUser(RegisterViewModel registerViewModel)
        {
            try
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

                // Log the success scenario
                _logger.LogInformation("User registered successfully: {Email}", registerViewModel.Email);
            }
            catch (Exception ex)
            {
                // Log the error scenario
                _logger.LogError(ex, "Error occurred while registering user.");
                throw;
            }
        }

        public async Task<bool> Authenticate(string? email, string? password)
        {
            try
            {
                var user = await _accountDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user != null)
                {
                    var isAuthenticated = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

                    if (isAuthenticated)
                        _logger.LogInformation("User logged in successfully: {Email}", email);
                    else
                        _logger.LogError("Invalid password. Please double-check your password and try again.{Email}", email);

                    return isAuthenticated;
                }
                else
                {
                    _logger.LogError("The email address '{email}' is not registered. Please check the spelling or register for a new account.", email);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while authenticating user.");
                throw;
            }
        }

        private static string HashPassword(string? password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
