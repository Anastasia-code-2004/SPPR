using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web253502Nikolaychik.UI.Services.ProductService;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.UI.Services.CategoryService;
using Microsoft.AspNetCore.Authorization;

namespace Web253502Nikolaychik.UI.Areas.Admin.Views.Product
{
    [Authorize(Policy = "admin")]
    public class EditModel(IProductService productService, ICategoryService categoryService) : PageModel
    {
        private readonly IProductService _productService = productService;
        private readonly ICategoryService _categoryService = categoryService;

        [BindProperty]
        public Commodity Commodity { get; set; } = default!;

        [BindProperty]
        public IFormFile? Image { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commodity = await _productService.GetProductByIdAsync(id.Value);
            if (commodity == null)
            {
                return NotFound();
            }
            Commodity = commodity.Data;
            var response = await _categoryService.GetCategoryListAsync();
            var categories = response.Data;
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var response = await _productService.UpdateProductAsync(Commodity.Id, Commodity, Image);
            if (!response.Successfull)
            {
                return NotFound(response);
            }

            return RedirectToPage("./Index");
        }
    }
}
