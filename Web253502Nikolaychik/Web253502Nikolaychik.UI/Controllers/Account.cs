using Microsoft.AspNetCore.Mvc;

namespace Web253502Nikolaychik.UI.Controllers
{
    public class Account : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LogOut()
        {
            return View();
        }
    }
}
