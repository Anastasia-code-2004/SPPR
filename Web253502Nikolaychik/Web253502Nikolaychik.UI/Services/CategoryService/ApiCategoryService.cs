using System.Text;
using System.Text.Json;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.Domain.Models;
using Web253502Nikolaychik.UI.Services.ProductService;

namespace Web253502Nikolaychik.UI.Services.CategoryService
{
    public class ApiCategoryService : ICategoryService
    {
        private HttpClient _httpClient;
        private JsonSerializerOptions _serializerOptions;
        private ILogger<ApiProductService> _logger;

        public ApiCategoryService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiProductService> logger)
        {
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _logger = logger;
        }
        public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            var urlString = new StringBuilder($"{_httpClient.BaseAddress.AbsoluteUri}Category");
            var response = await _httpClient.GetAsync(new Uri(urlString.ToString()));
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<ResponseData<List<Category>>>(_serializerOptions);
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"-----> Ошибка: {ex.Message}");
                    return ResponseData<List<Category>>.Error($"Ошибка: {ex.Message}");
                }
            }
            _logger.LogError($"-----> Данные не получены от сервера. Error: {response.StatusCode.ToString()}");
            return ResponseData<List<Category>>.Error($"Данные не получены от сервера. Error: {response.StatusCode.ToString()}");
        }
    }
}
