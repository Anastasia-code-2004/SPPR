using Microsoft.AspNetCore.Mvc;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.Domain.Models;
using Web253502Nikolaychik.UI.Models;
using Web253502Nikolaychik.UI.Services.CategoryService;
using Web253502Nikolaychik.UI.Services.ProductService;

namespace Web253502Nikolaychik.UI.Controllers
{
    public class Product(IProductService productService, ICategoryService categoryService) : Controller
    {
        public async Task<IActionResult> Index(string? category, int pageNo = 1)
        {
            var categoriesResponse = await categoryService.GetCategoryListAsync();

            if (!categoriesResponse.Successfull)
                return NotFound(categoriesResponse.ErrorMessage);


            var productsResponse = await productService.GetProductListAsync(category, pageNo);

            if(!productsResponse.Successfull)
                return NotFound(productsResponse.ErrorMessage);

            ViewBag.Categories = categoriesResponse.Data;
            ListModel<Commodity> commodities = new()
            {
                Items = productsResponse.Data.Items,
                CurrentPage = pageNo,
                TotalPages = productsResponse.Data.TotalPages
            };

            return View(commodities);
        }
    }
}
