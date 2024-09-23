using Microsoft.AspNetCore.Mvc;

namespace Web253502Nikolaychik.UI.Controllers
{
    
    public class Cart : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Add(int id, string returnUrl)
        {
            return Redirect(returnUrl);
        }
    }
}
