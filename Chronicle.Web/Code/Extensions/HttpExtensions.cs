using Microsoft.Net.Http.Headers;

namespace Chronicle.Web
{
    public static class HttpExtensions
    {
        public static string Referer(this IHttpContextAccessor httpContextAccessor)
        {
            var referer = httpContextAccessor.HttpContext?.Request?.Headers[HeaderNames.Referer];
            if (string.IsNullOrEmpty(referer)) return "";

            var uri = new Uri(referer!);
            return uri.GetComponents(UriComponents.PathAndQuery, UriFormat.UriEscaped);
        }

        public static string? PostedReferer(this IHttpContextAccessor httpContextAccessor)
        {
            var request = httpContextAccessor.HttpContext?.Request;
            if (request is not null && request.Method == "POST" && request.Form.ContainsKey("Referer"))
                return request.Form["Referer"].ToString();

            return null;
        }
    }
}
