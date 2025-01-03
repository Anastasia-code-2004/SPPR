﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing.Printing;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.Domain.Models;
using Web253502Nikolaychik.UI.Extensions;
using Web253502Nikolaychik.UI.Models;
using Web253502Nikolaychik.UI.Services.CategoryService;
using Web253502Nikolaychik.UI.Services.ProductService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web253502Nikolaychik.UI.Controllers
{
    [Route("Catalog")]
    public class Product(IProductService productService, ICategoryService categoryService) : Controller
    {
        [HttpGet("{category?}")]
        public async Task<IActionResult> Index([FromRoute] string? category, int pageNo = 1)
        {
           
            var categoriesResponse = await categoryService.GetCategoryListAsync();

            if (!categoriesResponse.Successfull)
                return NotFound(categoriesResponse.ErrorMessage);


            var productsResponse = await productService.GetProductListAsync(category, pageNo);

            if(!productsResponse.Successfull)
                return NotFound(productsResponse.ErrorMessage);

            ViewBag.Categories = categoriesResponse.Data;
            ViewBag.CurrentCategory = string.IsNullOrEmpty(category) ? "Все" : category;

            ListModel<Commodity> commodities = new()
            {
                Items = productsResponse.Data.Items,
                CurrentPage = pageNo,
                TotalPages = productsResponse.Data.TotalPages
            };
            
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", commodities);
            }

            return View(commodities);
        }
    }
}
