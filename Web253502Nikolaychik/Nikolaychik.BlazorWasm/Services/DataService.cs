using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Nikolaychik.BlazorWasm.Services
{
   public class DataService : IDataService
   {
        private readonly HttpClient _httpClient;
        private readonly int _pageSize;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly IAccessTokenProvider _tokenProvider;

        public event Action? DataLoaded;

        public List<Category> Categories { get; set; } = [];
        public List<Commodity> Commodities { get; set; } = [];
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public Category SelectedCategory { get; set; } = new Category();

        public DataService(HttpClient httpClient, IConfiguration configuration, IAccessTokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            _pageSize = int.Parse(configuration["ApiSettings:PageSize"]);
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _tokenProvider = tokenProvider;
        }

        public async Task GetProductListAsync(int pageNo = 1)
        {
            try
            {
                var route = new StringBuilder("Product");

                if (SelectedCategory?.NormalizedName is not null)
                {
                    route.Append($"/{SelectedCategory.NormalizedName}");
                };

                List<KeyValuePair<string, string>> queryData = [];
                if (pageNo > 1)
                {
                    queryData.Add(KeyValuePair.Create("pageNo", pageNo.ToString()));
                }
                if (!_pageSize.Equals(3))
                {
                    queryData.Add(KeyValuePair.Create("pageSize", _pageSize.ToString()));
                }
                if (queryData.Count > 0)
                {
                    route.Append(QueryString.Create(queryData));
                }

                var tokenRequest = await _tokenProvider.RequestAccessToken();
                if (tokenRequest.TryGetToken(out var token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Value);
                    var response = await _httpClient.GetAsync(route.ToString());
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<ResponseData<ListModel<Commodity>>>(_serializerOptions);
                        if (result?.Data != null)
                        {
                            Commodities = result.Data.Items;
                            TotalPages = result.Data.TotalPages;
                            CurrentPage = result.Data.CurrentPage;
                            Success = true;
                            ErrorMessage = string.Empty;
                            DataLoaded?.Invoke(); 
                        }
                        else
                        {
                            Success = false;
                            ErrorMessage = "Пустой ответ от сервера.";
                        }
                    }
                    else
                    {
                        Success = false;
                        ErrorMessage = $"Ошибка при запросе данных с сервера: {response.StatusCode}";
                    }
                }
                else
                {
                    Success = false;
                    ErrorMessage = "Не удалось получить JWT-токен.";
                }
            }
            catch (Exception ex)
            {
                Success = false;
                ErrorMessage = $"Ошибка при получении данных: {ex.Message}";
            }
        }

        public async Task GetCategoryListAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Category");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseData<List<Category>>>(_serializerOptions);
                    if (result?.Data != null)
                    {
                        Categories = result.Data;
                        Success = true;
                        DataLoaded?.Invoke();
                    }
                    else
                    {
                        Success = false;
                        ErrorMessage = "Пустой ответ от сервера.";
                    }
                }
                else
                {
                    Success = false;
                    ErrorMessage = $"Ошибка при запросе данных с сервера: {response.StatusCode}";
                }

            }
            catch (Exception ex)
            {
                Success = false;
                ErrorMessage = ex.Message;
            }
        }
   }
}

