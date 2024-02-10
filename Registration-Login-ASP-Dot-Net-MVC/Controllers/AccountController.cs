using Microsoft.AspNetCore.Mvc;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.AccountInterfaces;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.EmailInterface;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.OtpAuthenticationInterface;
using Registration_Login_ASP_Dot_Net_MVC.Models.AccountModel;
using Newtonsoft.Json;

namespace Registration_Login_ASP_Dot_Net_MVC.Controllers
{
    public class AccountController(IAccountInterface accountService, IOTPService otpService, IEmailService emailService) : Controller
    {
        private readonly IAccountInterface _accountService = accountService;
        private readonly IOTPService _otpService = otpService;
        private readonly IEmailService _emailService = emailService;

        [HttpGet]
        public IActionResult OTPVerification()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP(string otp)
        {
            // Retrieve the stored OTP and registration data JSON string from TempData
            var storedOTP = TempData["OTP"]?.ToString();
            var registerDataJson = TempData["RegisterDataJson"] as string;

            if (string.IsNullOrEmpty(storedOTP) || string.IsNullOrEmpty(registerDataJson))
            {
                // No OTP or registration data stored in TempData, handle the error
                return RedirectToAction("Error");
            }

            // Deserialize the registration data JSON string to RegisterViewModel object
            var registerViewModel = JsonConvert.DeserializeObject<RegisterViewModel>(registerDataJson);

            if (otp == storedOTP)
            {
                // OTP is valid, proceed to registration
                await _accountService.RegisterUser(registerViewModel);

                // Clear the OTP and registration data from TempData to prevent re-use
                TempData.Remove("OTP");
                TempData.Remove("RegisterDataJson");

                // Set success message
                TempData["SuccessMessage"] = "Registration successful! You are now logged in.";

                // Redirect to the home page
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Invalid OTP, display error message
                ModelState.AddModelError("otp", "Invalid OTP. Please try again.");
                return View("OTPVerification");
            }
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
                string otp = _otpService.GenerateOTP();
                await _emailService.SendOTPEmailAsync(registerViewModel.Email, otp);

                // Serialize the registration data to JSON string
                string registerDataJson = JsonConvert.SerializeObject(registerViewModel);

                // Store the registration data JSON string and OTP in TempData
                TempData["RegisterDataJson"] = registerDataJson;
                TempData["OTP"] = otp;

                // Set success message
                TempData["SuccessMessage"] = "OTP successfully sent! Please check your email.";

                // Redirect to OTP verification page
                return RedirectToAction("OTPVerification");
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
