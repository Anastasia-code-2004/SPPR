using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Web253502Nikolaychik.Domain.Entities;

namespace Web253502Nikolaychik.API.Data
{
    public class DbInitializer()
    {

        public static async Task SeedData(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSettings>>();


            var baseUrl = options.Value.BaseUrl;

            var categories = new List<Category>
            {
                new() { Name="Фрукты и Овощи", NormalizedName="fruits-vegetables" },
                new() { Name="Молочные продукты", NormalizedName="dairy-products" },
                new() { Name="Мясо и Птица", NormalizedName="meat-poultry" },
                new() { Name="Хлеб и Выпечка", NormalizedName="bread-bakery" },
                new() { Name="Напитки", NormalizedName="drinks" },
                new() { Name="Замороженные продукты", NormalizedName="frozen-food" }
            };

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(categories);
            }

            await context.SaveChangesAsync();

            var commodities = new List<Commodity>
            {
                new() { Name="Яблоки", Description="Свежие яблоки", Price=2.5m, Image=baseUrl + "/images/apple.jpg", ImageMimeType="jpg",
                         CategoryId=categories.Find(c => c.NormalizedName == "fruits-vegetables").Id },
                new() { Name="Молоко", Description="Молоко 2.5%", Price=1.2m, Image=baseUrl + "/images/milk.jpg", ImageMimeType="jpg",
                         CategoryId=categories.Find(c => c.NormalizedName == "dairy-products").Id },
                new() { Name="Куриное филе", Description="Куриное филе без кожи", Price=7.5m, Image=baseUrl + "/images/chicken-breast.jpg", ImageMimeType="jpg",
                         CategoryId=categories.Find(c => c.NormalizedName == "meat-poultry").Id },
                new() { Name="Хлеб", Description="Белый хлеб", Price=1.0m, Image=baseUrl + "/images/white-bread.jpg", ImageMimeType="jpg",
                         CategoryId=categories.Find(c => c.NormalizedName == "bread-bakery").Id }
            };

            if (!context.Commodities.Any())
            {
                context.Commodities.AddRange(commodities);
            }

            await context.SaveChangesAsync();
        }

    }
}
