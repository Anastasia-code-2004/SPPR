using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web253502Nikolaychik.API.Data;
using Web253502Nikolaychik.API.Services.ProductService;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.Domain.Models;

namespace Web253502Nikolaychik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        // GET: api/Product
        [HttpGet("category/{category?}")]
        public async Task<ActionResult<ResponseData<List<Commodity>>>> GetCommodities(string? category, int pageNo = 1, int pageSize = 3)
        {
            return Ok(await _productService.GetProductListAsync(category, pageNo, pageSize));
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseData<Commodity>>> GetCommodity(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);

            if (!result.Successfull)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        // PUT: api/Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseData<Commodity>>> PutCommodity(int id, Commodity commodity)
        {
            var commodityResult = await _productService.UpdateProductAsync(id, commodity);
            if (!commodityResult.Successfull)
            {
                return NotFound(commodityResult);
            }
            return Ok(commodityResult);
        }

        // POST: api/Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Commodity>> PostCommodity(Commodity commodity)
        {
            return Ok(await _productService.CreateProductAsync(commodity));
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseData<string>>> DeleteCommodity(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result.Successfull)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        private bool CommodityExists(int id)
        {
            var commodity = _productService.GetProductByIdAsync(id);
            return commodity.Result.Successfull;
        }
    }
}
