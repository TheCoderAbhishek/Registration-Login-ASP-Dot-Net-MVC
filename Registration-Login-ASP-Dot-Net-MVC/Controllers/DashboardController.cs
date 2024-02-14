using Microsoft.AspNetCore.Mvc;

namespace Registration_Login_ASP_Dot_Net_MVC.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
