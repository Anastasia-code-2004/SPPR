using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Web253502Nikolaychik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly string _imagePath;
        private readonly AppSettings _appSettings;
        public FilesController(IWebHostEnvironment webHost, IOptions<AppSettings> appSettings)
        {
            _imagePath = Path.Combine(webHost.WebRootPath, "Images");
            _appSettings = appSettings.Value;
        }
        [HttpPost]
        public async Task<IActionResult> SaveFile(IFormFile file)
        {
            if (file is null)
            {
                return BadRequest();
            }
            // путь к сохраняемому файлу
            var filePath = Path.Combine(_imagePath, file.FileName);
            var fileInfo = new FileInfo(filePath);
            // если такой файл существует, удалить его
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            // скопировать файл в поток
            using var fileStream = fileInfo.Create();
            await file.CopyToAsync(fileStream);
            // получить Url файла
            var host = HttpContext.Request.Host;
            var fileUrl = $"Https://{host}/Images/{file.FileName}";
            return Ok(fileUrl);
        }
        [HttpDelete]
        public IActionResult DeleteFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name is required.");
            }

            // Путь к файлу
            var filePath = Path.Combine(_imagePath, fileName);
            if (filePath == Path.Combine(_imagePath, _appSettings.DefaultImage))
            {
                return Ok();
            }

            // Проверяем, существует ли файл
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return NotFound("File not found.");
            }
            
            try
            {
                // Удаляем файл
                fileInfo.Delete();
                return Ok($"File '{fileName}' has been deleted successfully.");
            }
            catch (Exception ex)
            {
                // Если возникла ошибка при удалении
                return StatusCode(500, $"An error occurred while deleting the file: {ex.Message}");
            }
        }
    }
}
