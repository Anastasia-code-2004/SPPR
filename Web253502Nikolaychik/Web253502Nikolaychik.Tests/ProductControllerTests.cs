using NSubstitute;
using Web253502Nikolaychik.UI.Services.CategoryService;
using Web253502Nikolaychik.UI.Services.ProductService;
using Web253502Nikolaychik.UI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.Domain.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Primitives;

namespace Web253502Nikolaychik.Tests
{
    public class ProductControllerTests
    {
        private readonly ICategoryService _mockCategoryService;
        private readonly IProductService _mockProductService;
        private readonly Product _controller;

        public ProductControllerTests()
        {
            _mockCategoryService = Substitute.For<ICategoryService>();
            _mockProductService = Substitute.For<IProductService>();
            var controllerContext = new ControllerContext();
            var mockHttpContext = new Mock<HttpContext>();
            // Устанавливаем заголовки для имитации Ajax-запроса
            var headers = new Dictionary<string, StringValues>
            {
                ["x-requested-with"] = "XMLHttpRequest" // Имитация AJAX-запроса
            };

            mockHttpContext.Setup(c => c.Request.Headers)
                .Returns(new HeaderDictionary(headers));

            controllerContext.HttpContext = mockHttpContext.Object;

            // Передаем контроллеру
            _controller = new Product(_mockProductService, _mockCategoryService)
            {
                ControllerContext = controllerContext
            };
        }

        [Fact]
        public async Task Index_Returns404_WhenCategoryListFails()
        {
            // Arrange
            _mockCategoryService.GetCategoryListAsync()
                .Returns(Task.FromResult(new ResponseData<List<Category>>
                {
                    Successfull = false,
                    ErrorMessage = "Failed to load categories"
                }));

            // Act
            var result = await _controller.Index(null);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Failed to load categories", notFoundResult.Value);
        }

        [Fact]
        public async Task Index_Returns404_WhenProductListFails()
        {
            // Arrange
            // Имитация успешного получения категорий
            _mockCategoryService.GetCategoryListAsync()
                .Returns(Task.FromResult(new ResponseData<List<Category>>
                {
                    Successfull = true,
                    Data =
                    [
                         new Category { Id = 1, Name = "Category1" }
                    ]
                }));

            // Имитация неудачи при получении списка объектов
            _mockProductService.GetProductListAsync(Arg.Any<string>(), Arg.Any<int>())
                .Returns(Task.FromResult(new ResponseData<ListModel<Commodity>>
                {
                    Successfull = false,
                    ErrorMessage = "Failed to load products"
                }));

            // Act
            var result = await _controller.Index("Category1");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Failed to load products", notFoundResult.Value);
        }

        [Fact]
        public async Task Index_PopulatesViewBagWithCategories_WhenCategoryListIsSuccessful()
        {
            // Arrange: Создаем успешный ответ для списка категорий
            var categoryList = new List<Category>
            {
                new() { Id = 1, Name = "Category1", NormalizedName = "CATEGORY1" },
                new() { Id = 2, Name = "Category2", NormalizedName = "CATEGORY2" }
            };
            _mockCategoryService.GetCategoryListAsync()
                .Returns(Task.FromResult(new ResponseData<List<Category>>
                {
                    Successfull = true,
                    Data = categoryList
                }));

            // Создаем успешный ответ для списка продуктов
            var productList = new ListModel<Commodity>
            {
                Items =
                [
                    new Commodity { Id = 1, Name = "Product1" },
                    new Commodity { Id = 2, Name = "Product2" }
                ],
                CurrentPage = 1,
                TotalPages = 1
            };
            _mockProductService.GetProductListAsync(Arg.Any<string>(), Arg.Any<int>())
                .Returns(Task.FromResult(new ResponseData<ListModel<Commodity>>
                {
                    Successfull = true,
                    Data = productList
                }));

            // Act: Вызываем метод контроллера
            var result = await _controller.Index(null);

            // Assert: Проверяем содержимое ViewBag.Categories
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(_controller.ViewBag.Categories is List<Category>);
            var categories = _controller.ViewBag.Categories as List<Category>;
            Assert.NotNull(categories);
            Assert.Equal(2, categories.Count);
            Assert.Equal("Category1", categories[0].Name);
            Assert.Equal("Category2", categories[1].Name);
        }

        [Fact]
        public async Task Index_AssignsCorrectCurrentCategoryToViewBag()
        {
            // Arrange
            var categoryList = new List<Category>
            {
                new() { Name = "Category1", NormalizedName = "CATEGORY1" },
                new() { Name = "Category2", NormalizedName = "CATEGORY2" }
            };

            _mockCategoryService.GetCategoryListAsync()
                .Returns(Task.FromResult(ResponseData<List<Category>>.Success(categoryList)));

            _mockProductService.GetProductListAsync(Arg.Any<string>(), Arg.Any<int>())
                .Returns(Task.FromResult(ResponseData<ListModel<Commodity>>.Success(new ListModel<Commodity>
                {
                    Items = [],
                    CurrentPage = 1,
                    TotalPages = 1
                })));

            // Act - проверка с неуказанной категорией
            var resultWithoutCategory = await _controller.Index(null);

            // Assert
            Assert.IsType<ViewResult>(resultWithoutCategory);
            var viewResultWithoutCategory = resultWithoutCategory as ViewResult;
            Assert.NotNull(viewResultWithoutCategory);

            // Проверяем содержимое ViewBag для случая, когда категория не указана
            Assert.NotNull(_controller.ViewBag.CurrentCategory);
            Assert.Equal("Все", _controller.ViewBag.CurrentCategory);

            // Act - проверка с указанной категорией
            var category = "Category1";
            var resultWithCategory = await _controller.Index(category);

            // Assert
            Assert.IsType<ViewResult>(resultWithCategory);
            var viewResultWithCategory = resultWithCategory as ViewResult;
            Assert.NotNull(viewResultWithCategory);

            // Проверяем содержимое ViewBag для указанной категории
            Assert.NotNull(_controller.ViewBag.CurrentCategory);
            Assert.Equal(category, _controller.ViewBag.CurrentCategory);
        }

        [Fact]
        public async Task Index_PopulatesViewWithProductList_WhenCategoryListIsSuccessful()
        {
            // Arrange: Создаем успешный ответ для списка категорий
            var categoryList = new List<Category>
            {
                new() { Id = 1, Name = "Category1", NormalizedName = "CATEGORY1" },
                new() { Id = 2, Name = "Category2", NormalizedName = "CATEGORY2" }
            };
            _mockCategoryService.GetCategoryListAsync()
                .Returns(Task.FromResult(ResponseData<List<Category>>.Success(categoryList)));

            // Создаем успешный ответ для списка товаров
            var productList = new ListModel<Commodity>
            {
                Items =
                [
                    new Commodity { Id = 1, Name = "Product1" },
                    new Commodity { Id = 2, Name = "Product2" }
                ],
                CurrentPage = 1,
                TotalPages = 1
            };
            _mockProductService.GetProductListAsync(Arg.Any<string>(), Arg.Any<int>())
                .Returns(Task.FromResult(ResponseData<ListModel<Commodity>>.Success(productList)));

            // Act: Вызываем метод контроллера
            var result = await _controller.Index(null);

            // Assert: Проверяем, что результат является ViewResult
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            // Проверяем, что в модель передан правильный список товаров
            var model = viewResult.Model as ListModel<Commodity>;
            Assert.NotNull(model);
            Assert.Equal(2, model.Items.Count); // Убедитесь, что в модели два товара
            Assert.Equal("Product1", model.Items[0].Name); // Проверка первого товара
            Assert.Equal("Product2", model.Items[1].Name); // Проверка второго товара
        }

    }
}