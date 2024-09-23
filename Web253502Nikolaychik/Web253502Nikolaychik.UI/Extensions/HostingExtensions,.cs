using Web253502Nikolaychik.UI.Services.CategoryService;
using Web253502Nikolaychik.UI.Services.ProductService;

namespace Web253502Nikolaychik.UI.Extensions
{
    public static class HostingExtensions
    {
        public static void RegisterCustomServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICategoryService, MemoryCategoryService>();
            builder.Services.AddScoped<IProductService, MemoryProductService>();
        }
    }
}
