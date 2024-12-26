using Microsoft.AspNetCore.Mvc;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.UI.Extensions;

namespace Web253502Nikolaychik.UI.ViewComponents
{
    public class CartSummary : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Получаем объект корзины из сессии
            var cart = HttpContext.Session.Get<Cart>("Cart") ?? new Cart();

            // Передаем объект корзины в представление
            return View(cart);
        }
    }
}
