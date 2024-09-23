using Microsoft.AspNetCore.Mvc;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.Domain.Models;
using Web253502Nikolaychik.UI.Services.CategoryService;

namespace Web253502Nikolaychik.UI.Services.ProductService
{
    public class MemoryProductService : IProductService
    {
        List<Commodity> _commodities;
        private readonly List<Category> _categories;
        private readonly int _itemsPerPage;
        public MemoryProductService([FromServices] IConfiguration config, 
                                    ICategoryService categoryService)
        {
            _categories = categoryService.GetCategoryListAsync().Result.Data;
            _itemsPerPage = config.GetValue<int>("ItemsPerPage");
            SetupData();
        }
        private void SetupData()
        {
            _commodities =
            [
                new() {Id=1, Name="Яблоки", Description="Свежие яблоки", Price=2.5m, Image="apple", ImageMimeType="jpg",
                         CategoryId=_categories.Find(c => c.NormalizedName == "fruits-vegetables").Id},
                new() {Id=2, Name="Молоко", Description="Молоко 2.5%", Price=1.2m, Image="milk", ImageMimeType="jpg",
                         CategoryId=_categories.Find(c => c.NormalizedName == "dairy-products").Id},
                new() {Id=3, Name="Куриное филе", Description="Куриное филе без кожи", Price=7.5m, Image="chicken-breast", ImageMimeType="jpg",
                         CategoryId=_categories.Find(c => c.NormalizedName == "meat-poultry").Id},
                new() {Id=4, Name="Хлеб", Description="Белый хлеб", Price=1.0m, Image="white-bread", ImageMimeType="jpg",
                         CategoryId=_categories.Find(c => c.NormalizedName == "bread-bakery").Id},
            ];
        }
        public Task<ResponseData<Commodity>> CreateProductAsync(Commodity product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<string>> DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Commodity>> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<ListModel<Commodity>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            var category = _categories.FirstOrDefault(c => c.NormalizedName == categoryNormalizedName);
            var commodities = _commodities.Where(c => (category is null) || c.CategoryId == category?.Id).ToList();
            
            int totalPages = (int)Math.Ceiling(commodities.Count / (double)_itemsPerPage);

            commodities = commodities.Skip((pageNo - 1) * _itemsPerPage)
                                     .Take(_itemsPerPage)
                                     .ToList();

            return Task.FromResult(ResponseData<ListModel<Commodity>>.Success(new ListModel<Commodity>() { Items = commodities, 
                                                                                                           TotalPages = totalPages, CurrentPage = pageNo}));
        }

        public Task<ResponseData<Commodity>> UpdateProductAsync(int id, Commodity product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

    }
}
