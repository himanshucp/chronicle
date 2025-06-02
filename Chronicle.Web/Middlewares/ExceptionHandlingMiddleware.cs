using System.Net;
using System.Text.Json;

namespace Chronicle.Web.Middlewares
{
    /// <summary>
    /// Middleware for handling exceptions globally
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var response = new
            {
                message = _env.IsDevelopment()
                              ? exception.Message
                              : "An error occurred. Please try again later.",
                stackTrace = _env.IsDevelopment()
                              ? exception.StackTrace
                              : null        // still creates the property, just null in PROD
            };

            // Use the correct JsonNamingPolicy type
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    var json = JsonSerializer.Serialize(response, options);

                    // For API requests, return JSON
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = 500;                       // optional, but good to indicate server error
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(json);
                    }
                    else
                    {
                        // For web requests, redirect to error page
                        var msg = WebUtility.UrlEncode(response.message);
                        context.Response.Redirect($"/Home/Error?message={msg}");
                    }
                }
    }
}
