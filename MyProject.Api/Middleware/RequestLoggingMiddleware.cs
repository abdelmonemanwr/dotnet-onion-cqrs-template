using System.Security.Claims;

namespace MyProject.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            var logFilePath = $"Logs/request_{userId}_{DateTime.UtcNow:yyyyMMdd_HHmmssfff}.log";
            context.Items["LogFilePath"] = logFilePath;

            await _next(context);
        }
    }
}
