using Microsoft.EntityFrameworkCore;
using Web253502Nikolaychik.UI;
using Web253502Nikolaychik.UI.Extensions;
using Web253502Nikolaychik.UI.Services.CategoryService;
using Web253502Nikolaychik.UI.Services.ProductService;

var builder = WebApplication.CreateBuilder(args); 

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.RegisterCustomServices(); // Регистрация пользовательских сервисов.
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.Configure<UriData>(builder.Configuration.GetSection("UriData"));
var uriData = builder.Configuration.GetSection("UriData").Get<UriData>();

builder.Services.AddHttpClient<ICategoryService, ApiCategoryService>(opt =>
    opt.BaseAddress = new Uri(uriData.ApiUri));

builder.Services.AddHttpClient<IProductService, ApiProductService>(opt =>
    opt.BaseAddress = new Uri(uriData.ApiUri));

var app = builder.Build(); // Здесь создается экземпляр приложения (app),
                           // который включает в себя конвейер обработки запросов.

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run(); // Aктивирует конвейер обработки запросов.
           // На этом этапе приложение начинает принимать и обрабатывать входящие HTTP-запросы.
