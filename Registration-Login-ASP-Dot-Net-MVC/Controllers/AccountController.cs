using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.AccountInterfaces;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.EmailInterface;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.OtpInterface;
using Registration_Login_ASP_Dot_Net_MVC.Models.AccountModel;
using Registration_Login_ASP_Dot_Net_MVC.Models.reCaptchaModel;
using Registration_Login_ASP_Dot_Net_MVC.Services.EmailService;
using Registration_Login_ASP_Dot_Net_MVC.Services.OtpService;

namespace Registration_Login_ASP_Dot_Net_MVC.Controllers
{
    public class AccountController(IAccountInterface accountService, ILogger<AccountController> logger, IOtpInterface otpInterface, IEmailInterface emailInterface) : Controller
    {
        private readonly IAccountInterface _accountService = accountService;
        private readonly ILogger<AccountController> _logger = logger;
        private readonly IOtpInterface _otpInterface = otpInterface;
        private readonly IEmailInterface _emailInterface = emailInterface;

        [HttpGet]
        public IActionResult OTPVerification()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP(string otp)
        {
            var storedOTP = TempData["OTP"]?.ToString();
            var registerDataJson = TempData["RegisterDataJson"] as string;

            if (string.IsNullOrEmpty(storedOTP) || string.IsNullOrEmpty(registerDataJson))
            {
                return RedirectToAction("Error");
            }

            var registerViewModel = JsonConvert.DeserializeObject<RegisterViewModel>(registerDataJson);

            if (registerViewModel == null)
            {
                ModelState.AddModelError("", "Registration data is invalid.");
                return View("OTPVerification");
            }

            if (otp == storedOTP)
            {
                await _accountService.RegisterUser(registerViewModel);

                TempData.Remove("OTP");
                TempData.Remove("RegisterDataJson");

                TempData["SuccessMessage"] = "Registration successful! You are now logged in.";

                return RedirectToAction("Index", "Home");
            }
            else
            {
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
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel, string recaptchaToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var httpClient = new HttpClient();
                    var response = await httpClient.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret=[your_secret_key]&response={recaptchaToken}");

                    var result = JsonConvert.DeserializeObject<RecaptchaResponse>(response);

                    if (result == null)
                    {
                        ViewData["ErrorMessage"] = "reCAPTCHA validation failed. Please try again.";
                        return View(registerViewModel);
                    }

                    if (!result.Success)
                    {
                        ViewData["ErrorMessage"] = "reCAPTCHA validation failed. Please try again.";
                        return View(registerViewModel);
                    }

                    string otp = _otpInterface.GenerateOTP();

                    if (registerViewModel.Email != null)
                    {
                        await _emailInterface.SendOTPEmailAsync(registerViewModel.Email, otp);
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "Email address is required.");
                        return View(registerViewModel);
                    }

                    if (await _accountService.IsEmailAlreadyRegistered(registerViewModel.Email))
                    {
                        ModelState.AddModelError("Email", "Email is already registered.");
                        _logger.LogWarning("Email '{Email}' is already registered.", registerViewModel.Email);
                        return View(registerViewModel);
                    }

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
                catch (Exception ex)
                {
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
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string recaptchaToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var httpClient = new HttpClient();
                    var response = await httpClient.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret=[your_secret_key]&response={recaptchaToken}");

                    var result = JsonConvert.DeserializeObject<RecaptchaResponse>(response);

                    if (result == null)
                    {
                        ViewData["ErrorMessage"] = "reCAPTCHA validation failed. Please try again.";
                        return View(loginViewModel);
                    }

                    if (!result.Success)
                    {
                        ViewData["ErrorMessage"] = "reCAPTCHA validation failed. Please try again.";
                        return View(loginViewModel);
                    }

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
