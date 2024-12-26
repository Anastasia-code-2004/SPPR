using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web253502Nikolaychik.UI.Services.ProductService;
using Web253502Nikolaychik.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Web253502Nikolaychik.UI.Areas.Admin.Views.Product
{
    [Authorize(Policy = "admin")]
    public class DeleteModel(IProductService productService) : PageModel
    {
        private readonly IProductService _productService = productService;

        [BindProperty]
        public Commodity Commodity { get; set; } = default!;

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
            else
            {
                Commodity = commodity.Data;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _productService.DeleteProductAsync(id.Value);

            return RedirectToPage("./Index");
        }
    }
}
