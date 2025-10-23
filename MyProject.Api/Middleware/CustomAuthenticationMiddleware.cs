using System.Text.Json;

namespace MyProject.Api.Middleware
{
    public class CustomAuthenticationMiddleware
    {
        private readonly RequestDelegate next;

        public CustomAuthenticationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await next(context);
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    statusCode = 401,
                    statusMessage = "Unauthorized",
                    isSuccess = false,
                    totalCount = 0,
                    data = (object?)null
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    statusCode = 403,
                    statusMessage = "Forbidden",
                    isSuccess = false,
                    totalCount = 0,
                    data = (object?)null
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }

    }
}
