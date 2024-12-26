using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web253502Nikolaychik.UI.Services.CategoryService;
using Web253502Nikolaychik.UI.Services.ProductService;
using Web253502Nikolaychik.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Web253502Nikolaychik.UI.Areas.Admin.Views.Product
{
    [Authorize(Policy = "admin")]
    public class CreateModel(IProductService productService, ICategoryService categoryService) : PageModel
    {
        private readonly IProductService _productService = productService;
        private readonly ICategoryService _categoryService = categoryService;

        public async Task<IActionResult> OnGet()
        {
            var response = await _categoryService.GetCategoryListAsync();
            var categories = response.Data;
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Commodity Commodity { get; set; } = default!;

        [BindProperty]
        public IFormFile? Image { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _productService.CreateProductAsync(Commodity, Image);

            return RedirectToPage("./Index");
        }
    }
}
