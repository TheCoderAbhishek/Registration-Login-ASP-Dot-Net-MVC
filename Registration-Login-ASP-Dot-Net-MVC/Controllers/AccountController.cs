using Microsoft.AspNetCore.Mvc;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.AccountInterfaces;
using Registration_Login_ASP_Dot_Net_MVC.Models.AccountModel;
using System.Threading.Tasks;

namespace Registration_Login_ASP_Dot_Net_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountInterface _accountService;

        public AccountController(IAccountInterface accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                // Check if email is already registered
                if (await _accountService.IsEmailAlreadyRegistered(registerViewModel.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(registerViewModel);
                }

                // Register the user
                await _accountService.RegisterUser(registerViewModel);

                // Redirect to the Index action of HomeController
                return RedirectToAction("Index", "Home");
            }

            return View(registerViewModel);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                // Call your authentication service to verify login credentials
                var isAuthenticated = await _accountService.Authenticate(loginViewModel.Email, loginViewModel.Password);

                if (isAuthenticated)
                {
                    // Redirect authenticated user to a dashboard or home page
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // If authentication fails, add a model error and return the login view
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginViewModel);
                }
            }

            // If model state is not valid, return the login view with validation errors
            return View(loginViewModel);
        }
    }
}