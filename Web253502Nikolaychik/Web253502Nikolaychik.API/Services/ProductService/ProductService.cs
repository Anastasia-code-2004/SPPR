using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Drawing.Printing;
using Web253502Nikolaychik.API.Data;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.Domain.Models;

namespace Web253502Nikolaychik.API.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _baseUrl;
        private const int _maxPageSize = 20;

        public ProductService(AppDbContext context, IWebHostEnvironment webHostEnvironment, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _baseUrl = appSettings.Value.BaseUrl; 
        }

        public async Task<ResponseData<Commodity>> CreateProductAsync(Commodity product)
        {
            var commodity = _context.Commodities.Add(product);
            await _context.SaveChangesAsync();
            return ResponseData<Commodity>.Success(commodity.Entity);
        }

        public async Task<ResponseData<string>> DeleteProductAsync(int id)
        {
            var query = _context.Commodities.AsQueryable();
            var commodity = await query.FirstOrDefaultAsync(c => c.Id.Equals(id));
            if (commodity is not null)
            {
                _context.Commodities.Remove(commodity);
                await _context.SaveChangesAsync();
                return ResponseData<string>.Success("Product deleted");
            }
            return ResponseData<string>.Error("No such product");
        }

        public async Task<ResponseData<Commodity>> GetProductByIdAsync(int id)
        {
            var query = _context.Commodities.AsQueryable();
            var commodity = await query.FirstOrDefaultAsync(c => c.Id.Equals(id));
            
            if (commodity is null)
            {
                return ResponseData<Commodity>.Error("No such product", commodity);
            }
            return ResponseData<Commodity>.Success(commodity);
        }

        public async Task<ResponseData<ListModel<Commodity>>> GetProductListAsync(string? categoryNormalizedName, 
                                                                                    int pageNo = 1, int pageSize = 3)
        {
            if (pageSize > _maxPageSize)
                pageSize = _maxPageSize;

            var query = _context.Commodities.AsQueryable();
            var dataList = new ListModel<Commodity>();

            query = query.Where(c => categoryNormalizedName == null || c.Category.NormalizedName.Equals(categoryNormalizedName));
            var count = await query.CountAsync(); 

            if (count == 0)
            {
                return ResponseData<ListModel<Commodity>>.Success(dataList);
            }

            int totalPages = (int)Math.Ceiling(count / (double)pageSize);

            if (pageNo > totalPages) // если количество товаров между запросами изменилось, их стало меньше и номер запрашиваемой страницы уже не существует
                return ResponseData<ListModel<Commodity>>.Error("No such page");

            dataList.Items = await query
                                    .OrderBy(c => c.Id)
                                    .Skip((pageNo - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();
            dataList.CurrentPage = pageNo;
            dataList.TotalPages = totalPages;
            return ResponseData<ListModel<Commodity>>.Success(dataList);
        }

        public async Task<ResponseData<string>> SaveImageAsync(int id, IFormFile formFile)
        {
            var query = _context.Commodities.AsQueryable();
            var commodity = await query.FirstOrDefaultAsync(c => c.Id.Equals(id));
            
            if (commodity is null)
            {
                return ResponseData<string>.Error("No such product");
            }


            if (formFile is null || formFile.Length == 0)
            {
                return ResponseData<string>.Error("Invalid file");
            }


            var fileExtension = Path.GetExtension(formFile.FileName);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            if (!allowedExtensions.Contains(fileExtension.ToLower()))
            {
                return ResponseData<string>.Error("Invalid image format");
            }
            var fileName = Path.GetFileName(formFile.FileName);
            var savePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);


            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            commodity.Image = _baseUrl + formFile.FileName;
            _context.Commodities.Update(commodity);
            await _context.SaveChangesAsync();
            return ResponseData<string>.Success(commodity.Image);
        }

        public async Task<ResponseData<Commodity>> UpdateProductAsync(int id, Commodity product)
        {
            var query = _context.Commodities.AsQueryable();
            var commodity = await query.FirstOrDefaultAsync(c => c.Id.Equals(id));
            if (commodity is not null)
            {
                commodity.Name = product.Name;
                commodity.Description = product.Description;
                commodity.Price = product.Price;
                commodity.CategoryId = product.CategoryId;
                await _context.SaveChangesAsync();
                return ResponseData<Commodity>.Success(commodity);
            }
            return ResponseData<Commodity>.Error("No such product", commodity);
        }
    }
}
