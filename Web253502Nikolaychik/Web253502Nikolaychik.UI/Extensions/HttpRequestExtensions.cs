namespace Web253502Nikolaychik.UI.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            return request.Headers.XRequestedWith.ToString().ToLower().Equals("xmlhttprequest");
        }
    }
}
