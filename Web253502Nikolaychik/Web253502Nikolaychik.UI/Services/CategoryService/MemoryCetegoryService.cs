using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.Domain.Models;

namespace Web253502Nikolaychik.UI.Services.CategoryService
{
    public class MemoryCategoryService : ICategoryService
    {
        public Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            var categories = new List<Category>
            {
                new() {Id=1, Name="Фрукты и Овощи",
                             NormalizedName="fruits-vegetables"},
                new() {Id=2, Name="Молочные продукты",
                             NormalizedName="dairy-products"},
                new() {Id=3, Name="Мясо и Птица", 
                             NormalizedName="meat-poultry"},
                new() {Id=4, Name="Хлеб и Выпечка",
                             NormalizedName="bread-bakery"},
                new() {Id=5, Name="Напитки",
                             NormalizedName="drinks"},
                new() {Id=6, Name="Замороженные продукты",
                             NormalizedName="frozen-food"}
            };
            var result = ResponseData<List<Category>>.Success(categories);
            return Task.FromResult(result);
        }
    }
}
