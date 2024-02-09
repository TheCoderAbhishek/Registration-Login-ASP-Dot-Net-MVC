using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Registration_Login_ASP_Dot_Net_MVC.Data;

namespace Registration_Login_ASP_Dot_Net_MVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountDbContext _accountDbContext;

        public AccountController(AccountDbContext accountDbContext)
        {
            _accountDbContext = accountDbContext;
        }

        // GET: api/Account
        [HttpGet]
        public async Task<ActionResult> GetUsers()
        {
            var users = await _accountDbContext.Users.ToListAsync();
            return Ok(users);
        }
    }
}
