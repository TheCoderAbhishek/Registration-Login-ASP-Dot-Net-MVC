using Microsoft.AspNetCore.Mvc;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.AccountInterfaces;
using Registration_Login_ASP_Dot_Net_MVC.Models.AccountModel;

namespace Registration_Login_ASP_Dot_Net_MVC.Controllers
{
    public class AccountController(IAccountInterface accountService, ILogger<AccountController> logger) : Controller
    {
        private readonly IAccountInterface _accountService = accountService;
        private readonly ILogger<AccountController> _logger = logger;

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
                try
                {
                    // Check if email is already registered
                    if (await _accountService.IsEmailAlreadyRegistered(registerViewModel.Email))
                    {
                        ModelState.AddModelError("Email", "Email is already registered.");

                        // Log the scenario
                        _logger.LogWarning("Email '{Email}' is already registered.", registerViewModel.Email);

                        return View(registerViewModel);
                    }

                    // Register the user
                    await _accountService.RegisterUser(registerViewModel);

                    // Log the success scenario
                    _logger.LogInformation("User registered successfully: {Email}", registerViewModel.Email);

                    // Set a success message
                    TempData["SuccessMessage"] = "Registration successful! You can now login.";

                    // Redirect to the Index action of HomeController
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    // Log the error scenario
                    _logger.LogError(ex, "Error occurred while registering user.");
                    ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                    return View(registerViewModel);
                }
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
                try
                {
                    var isEmailRegistered = await _accountService.IsEmailAlreadyRegistered(loginViewModel.Email);

                    if (!isEmailRegistered)
                    {
                        _logger.LogWarning($"Email '{loginViewModel.Email}' is not registered.");
                        ViewData["ErrorMessage"] = "Email address not found. Please register an account.";
                        return View(loginViewModel);
                    }

                    var isAuthenticated = await _accountService.Authenticate(loginViewModel.Email, loginViewModel.Password);

                    if (isAuthenticated)
                    {
                        _logger.LogInformation("User logged in successfully: {Email}", loginViewModel.Email);
                        // Set a success message
                        TempData["SuccessMessage"] = "Login successful.";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        _logger.LogWarning("Invalid login attempt for email: {Email}", loginViewModel.Email);
                        ViewData["ErrorMessage"] = "Invalid password. Please double-check your password and try again.";
                        return View(loginViewModel);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while logging in user.");
                    ViewData["ErrorMessage"] = "An error occurred while processing your request.";
                    return View(loginViewModel);
                }
            }

            return View(loginViewModel);
        }
    }
}
