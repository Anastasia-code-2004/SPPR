using Microsoft.AspNetCore.Mvc;
using Web253502Nikolaychik.UI.Models;

namespace Web253502Nikolaychik.UI.Controllers
{
    public class Home : Controller
    {
        public IActionResult Index()
        {
            var items = new List<ListDemo>
            {
                new() { Id = 1, Name = "Первый" },
                new() { Id = 2, Name = "Второй" },
                new() { Id = 3, Name = "Третий" }
            };
            ViewData["Lab"] = "Лабораторная работа 2";
            return View(items);
        }
    }
}
