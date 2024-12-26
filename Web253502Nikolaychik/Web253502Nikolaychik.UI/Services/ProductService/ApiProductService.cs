using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.Domain.Models;
using Web253502Nikolaychik.UI.Services.Authentication;
using Web253502Nikolaychik.UI.Services.FileService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web253502Nikolaychik.UI.Services.ProductService
{
    public class ApiProductService : IProductService
    {
        private HttpClient _httpClient;
        private IFileService _fileService;
        private string? _pageSize;
        private JsonSerializerOptions _serializerOptions;
        private ILogger<ApiProductService> _logger;
        private ITokenAccessor _tokenAccessor;

        public ApiProductService(HttpClient httpClient, IConfiguration configuration, 
            ILogger<ApiProductService> logger, IFileService fileService, ITokenAccessor tokenAccessor)
        {
            _httpClient = httpClient;
            _pageSize = configuration.GetSection("ItemsPerPage").Value;
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _logger = logger;
            _fileService = fileService;
            _tokenAccessor = tokenAccessor;
        }
        public async Task<ResponseData<Commodity>> CreateProductAsync(Commodity product, IFormFile? formFile)
        {
            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);
            // Сохранить файл изображения
            if (formFile != null)
            {
                var imageUrl = await _fileService.SaveFileAsync(formFile);
                var extension = Path.GetExtension(formFile.FileName);
                // Добавить в объект Url изображения
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    product.Image = imageUrl;
                    product.ImageMimeType = extension;
                }
            }
            var uri = new Uri(_httpClient.BaseAddress.AbsoluteUri + "Product");
            
            var response = await _httpClient.PostAsJsonAsync(uri, product, _serializerOptions);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var data = await response.Content.ReadFromJsonAsync<ResponseData<Commodity>>(_serializerOptions);
                    return data;
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"-----> Ошибка: {ex.Message}");
                    return ResponseData<Commodity>.Error($"Ошибка: {ex.Message}");
                }
            }
            _logger.LogError($"Объект не добавлен. Error: {response.StatusCode.ToString()}");
            return ResponseData<Commodity>.Error($"Объект не добавлен. Error:{response.StatusCode}");
        }

        public async Task<ResponseData<string>> DeleteProductAsync(int id)
        {
            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);
            var uri = new Uri(_httpClient.BaseAddress.AbsoluteUri + "Product/" + $"{id}");

            var product = await GetProductByIdAsync(id);
            var uriImage = new Uri(product.Data.Image);
            var fileName = Path.GetFileName(uriImage.LocalPath);
            await _fileService.DeleteFileAsync(fileName);

            var response = await _httpClient.DeleteAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<ResponseData<string>>(_serializerOptions);
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"-----> Ошибка: {ex.Message}");
                    return ResponseData<string>.Error($"Ошибка: {ex.Message}");
                }
            }
            _logger.LogError($"-----> Объект не удален. Error: {response.StatusCode.ToString()}");
            return ResponseData<string>.Error($"Объект не удален. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<Commodity>> GetProductByIdAsync(int id)
        {
            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);

            var uri = new Uri(_httpClient.BaseAddress.AbsoluteUri + "Product/" + $"{id}");
            
            var response = await _httpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<ResponseData<Commodity>>(_serializerOptions);
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"-----> Ошибка: {ex.Message}");
                    return ResponseData<Commodity>.Error($"Ошибка: {ex.Message}");
                }
            }
            _logger.LogError($"-----> Данные не получены от сервера. Error: {response.StatusCode.ToString()}");
            return ResponseData<Commodity>.Error($"Данные не получены от сервера. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<ListModel<Commodity>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);
            // подготовка URL запроса
            var urlString = new StringBuilder($"{_httpClient.BaseAddress.AbsoluteUri}Product");
            // добавить категорию в маршрут
            if (categoryNormalizedName != null)
            {
                urlString.Append($"/{categoryNormalizedName}");
            }
            var queryString = new List<string>();
            if (pageNo > 1)
            {
                queryString.Add($"pageNo={pageNo}");
            }
            if (!int.Parse(_pageSize).Equals(3))
            {
                queryString.Add($"pageSize={_pageSize}");
            }
            if (queryString.Count > 0)
            {
                urlString.Append("?" + string.Join("&", queryString));
            }
            // выполнение запроса
            var response = await _httpClient.GetAsync(new Uri(urlString.ToString()));
            if (response.IsSuccessStatusCode)
            {
                try 
                {
                    return await response.Content.ReadFromJsonAsync<ResponseData<ListModel<Commodity>>>(_serializerOptions);
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"-----> Ошибка: {ex.Message}");
                    return ResponseData<ListModel<Commodity>>.Error($"Ошибка: {ex.Message}");
                }

            }
            _logger.LogError($"-----> Данные не получены от сервера. Error: {response.StatusCode.ToString()}");
            return ResponseData<ListModel<Commodity>>.Error($"Данные не получены от сервера. Error: {response.StatusCode.ToString()}, url: {urlString}");
        }

        public async Task<ResponseData<Commodity>> UpdateProductAsync(int id, Commodity product, IFormFile? formFile)
        {
            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);
            var uri = new Uri(_httpClient.BaseAddress.AbsoluteUri + "Product/" + $"{id}");
            if(formFile != null)
            {
                var imageUrl = await _fileService.SaveFileAsync(formFile);
                var extension = Path.GetExtension(formFile.FileName);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    product.Image = imageUrl;
                    product.ImageMimeType = extension;
                }
            }
            
            var response = await _httpClient.PutAsJsonAsync(uri, product, _serializerOptions);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var data = await response.Content.ReadFromJsonAsync<ResponseData<Commodity>>(_serializerOptions);
                    return data;
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"-----> Ошибка: {ex.Message}");
                    return ResponseData<Commodity>.Error($"Ошибка: {ex.Message}");
                }
            }
            _logger.LogError($"Объект не обновлен. Error: {response.StatusCode.ToString()}");
            return ResponseData<Commodity>.Error($"Объект не обновлен. Error:{response.StatusCode}");
        }
    }
}
