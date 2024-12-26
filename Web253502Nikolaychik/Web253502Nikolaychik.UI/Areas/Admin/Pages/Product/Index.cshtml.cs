using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.UI.Services.ProductService;

namespace Web253502Nikolaychik.UI.Areas.Admin.Views.Product
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        public IList<Commodity> Commodity { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public int PageNo { get; set; } = 1;
        public int TotalPages { get; private set; }

        public async Task OnGetAsync()
        {
            var response = await _productService.GetProductListAsync(null, PageNo);
            TotalPages = response.Data.TotalPages;
            Commodity.AddRange(response.Data.Items);
        }
    }
}
