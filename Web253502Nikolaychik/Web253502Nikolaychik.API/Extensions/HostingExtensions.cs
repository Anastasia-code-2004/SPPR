using Web253502Nikolaychik.API.Services.CategoryService;
using Web253502Nikolaychik.API.Services.ProductService;

namespace Web253502Nikolaychik.API.Extensions
{
    public static class HostingExtensions
    {
        public static void RegisterCustomServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, ProductService>();
        }
    }
}
