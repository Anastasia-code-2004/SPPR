using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.UI.Services.ProductService;


namespace Web253502Nikolaychik.UI.Controllers
{
    
    public class Cart(IProductService productService, Web253502Nikolaychik.Domain.Entities.Cart cart) : Controller
    {
        public IActionResult Index()
        {
            return View(cart);
        }

        [Authorize]
        [Route("[controller]/add/{id:int}")]
        public async Task<ActionResult> Add(int id, string returnUrl)
        {
            var data = await productService.GetProductByIdAsync(id);
            if (data.Successfull)
            {
                cart.AddToCart(data.Data);
            }
            return Redirect(returnUrl);
        }

        [Authorize]
        [HttpPost]
        [Route("[controller]/remove/{id:int}")]
        public IActionResult Remove(int id, string returnUrl)
        {
            cart.RemoveItem(id);
            return Redirect(returnUrl);
        }
    }
}
