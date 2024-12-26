using Serilog;

namespace Web253502Nikolaychik.UI.HelperClasses
{
    public class ErrorLoggingMiddleware(RequestDelegate next)
    {

        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            // Пропускаем запрос дальше по конвейеру
            await _next(context);

            // Проверяем код состояния после выполнения запроса
            if (context.Response.StatusCode < 200 || context.Response.StatusCode >= 300)
            {
                // Логируем URL и статус-код
                Log.Information("---> request {Url} returns {StatusCode}", context.Request.Path, context.Response.StatusCode);
            }
        }
    }

}
