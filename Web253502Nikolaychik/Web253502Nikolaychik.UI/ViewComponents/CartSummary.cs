using Microsoft.AspNetCore.Mvc;

namespace Web253502Nikolaychik.UI.ViewComponents
{
    public class CartSummary : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
