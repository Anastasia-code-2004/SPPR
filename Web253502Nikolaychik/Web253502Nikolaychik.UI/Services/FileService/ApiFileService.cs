
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Web253502Nikolaychik.UI.Services.Authentication;

namespace Web253502Nikolaychik.UI.Services.FileService
{
    public class ApiFileService : IFileService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenAccessor _tokenAccessor;
        public ApiFileService(HttpClient httpClient, ITokenAccessor tokenAccessor)
        {
            _httpClient = httpClient;
            //_httpContext = httpContextAccessor.HttpContext;
            _tokenAccessor = tokenAccessor;
        }
        public async Task DeleteFileAsync(string fileName)
        {
            await _httpClient.DeleteAsync($"?fileName={fileName}");

        }

        public async Task<string> SaveFileAsync(IFormFile formFile)
        {
            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);
            // Создать объект запроса
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post
            };
            // Сформировать случайное имя файла, сохранив расширение
            var extension = Path.GetExtension(formFile.FileName);
            var newName = Path.ChangeExtension(Path.GetRandomFileName(), extension);
            // Создать контент, содержащий поток загруженного файла
            var content = new MultipartFormDataContent(); // специальный класс, который формирует
                                                          // HTTP-запрос в формате multipart/form-data
            var streamContent = new StreamContent(formFile.OpenReadStream());
            content.Add(streamContent, "file", newName);
            // Поместить контент в запрос
            request.Content = content;
            // Отправить запрос к API
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return String.Empty;
        }
    }
}
