using Web253502Nikolaychik.API.Data;
using Web253502Nikolaychik.API.Services.ProductService;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.Domain.Models;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Web253502Nikolaychik.API;

namespace Web253502Nikolaychik.Tests
{
    public class ProductServiceTests
    {
        private readonly AppDbContext _context;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly Mock<IOptions<AppSettings>> _mockAppSettings;
        private readonly ProductService _service;

        // Конструктор для инициализации контекста, моков и сервиса
        public ProductServiceTests()
        {
            // Создание контекста базы данных с тестовыми данными
            _context = CreateContext();

            // Инициализация моков
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockAppSettings = new Mock<IOptions<AppSettings>>();

            _mockAppSettings.Setup(options => options.Value).Returns(new AppSettings { BaseUrl = "http://localhost" });

            // Создание экземпляра ProductService
            _service = new ProductService(_context, _mockWebHostEnvironment.Object, _mockAppSettings.Object);
        }

        // Метод для создания контекста с тестовыми данными
        public static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                          .UseInMemoryDatabase("TestDatabase")
                          .Options;
            var context = new AppDbContext(options);

            if (!context.Commodities.Any()) // Добавление тестовых данных только если они еще не добавлены
            {
                context.Commodities.AddRange(new List<Commodity>
                {
                    new() { Id = 1, Name = "Product 1", CategoryId = 1, Price = 10, Description = "Description for product 1" },
                    new() { Id = 2, Name = "Product 2", CategoryId = 1, Price = 20, Description = "Description for product 2" },
                    new() { Id = 3, Name = "Product 3", CategoryId = 1, Price = 30, Description = "Description for product 3" },
                    new() { Id = 4, Name = "Product 4", CategoryId = 2, Price = 40, Description = "Description for product 4" }
                });
                context.SaveChanges();
            }

            return context;
        }

        [Fact]
        public void ServiceReturnsFirstPageOfThreeItems()
        {
            var result = _service.GetProductListAsync(null).Result;
            Assert.IsType<ResponseData<ListModel<Commodity>>>(result);
            Assert.True(result.Successfull);
            Assert.Equal(1, result.Data.CurrentPage);
            Assert.Equal(3, result.Data.Items.Count);
            Assert.Equal(2, result.Data.TotalPages);
            Assert.Equal(_context.Commodities.First(), result.Data.Items[0]);
        }

        [Fact]
        public async void ChooseRightPage()
        {
            var result = await _service.GetProductListAsync(null, pageNo: 2, pageSize: 3);
            Assert.IsType<ResponseData<ListModel<Commodity>>>(result);
            Assert.True(result.Successfull);
            Assert.Equal(2, result.Data.CurrentPage);
            Assert.Single(result.Data.Items);
            Assert.Equal(2, result.Data.TotalPages);
            Assert.Equal(_context.Commodities.OrderBy(c => c.Id).Skip(3).First(), result.Data.Items[0]);
        }

        [Fact]
        public async Task ServiceReturnsFilteredListForExistingCategory()
        {
            // Arrange: создаем категорию и добавляем товары в эту категорию
            var categoryName = "Category 1"; // Убедитесь, что такая категория существует в контексте
            var categoryId = 1; // Идентификатор категории, который должен быть связан с товаром

            // Добавляем товары с категориями в базу данных, если они еще не добавлены
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Id = categoryId, Name = categoryName, NormalizedName = categoryName });
                _context.SaveChanges();
            }
            
            // Act: фильтруем товары по существующей категории
            var result = await _service.GetProductListAsync(categoryName, pageNo: 1, pageSize: 3);

            // Assert: проверяем, что результат содержит только товары из указанной категории
            Assert.IsType<ResponseData<ListModel<Commodity>>>(result);
            Assert.True(result.Successfull);
            Assert.Equal(3, result.Data.Items.Count); // Ожидаем 3 товара в списке, так как их столько в категории
            Assert.All(result.Data.Items, item => Assert.Equal(categoryId, item.CategoryId)); 
        }
        
        [Fact]
        public async Task ServiceLimitsPageSizeToMaxValue()
        {
            var result = await _service.GetProductListAsync(null, pageNo: 1, pageSize: 50);  // pageSize больше _maxPageSize

            // Assert: проверяем, что размер страницы был ограничен
            Assert.IsType<ResponseData<ListModel<Commodity>>>(result);
            Assert.True(result.Successfull);
            Assert.Equal(4, result.Data.Items.Count); 
        }
        
        [Fact]
        public async Task ServiceReturnsErrorWhenPageNumberExceedsTotalPages()
        {
            var result = await _service.GetProductListAsync(null, pageNo: 3, pageSize: 3);

            // Assert: проверяем, что результат содержит ошибку с соответствующим сообщением
            Assert.IsType<ResponseData<ListModel<Commodity>>>(result);
            Assert.False(result.Successfull); // Ожидаем, что Success будет false
            Assert.Equal("No such page", result.ErrorMessage); // Ожидаем, что сообщение будет "No such page"
        }

    }
}
