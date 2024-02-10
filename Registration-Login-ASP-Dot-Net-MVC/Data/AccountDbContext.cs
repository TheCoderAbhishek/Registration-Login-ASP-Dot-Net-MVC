using Microsoft.EntityFrameworkCore;
using static Registration_Login_ASP_Dot_Net_MVC.Models.AccountModel.Users;

namespace Registration_Login_ASP_Dot_Net_MVC.Data
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
        {
        }

        // DbSet representing the Users table in the database
        public DbSet<User> Users { get; set; }
    }
}
