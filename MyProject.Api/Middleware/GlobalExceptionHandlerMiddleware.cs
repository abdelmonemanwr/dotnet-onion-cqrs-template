using Microsoft.Data.SqlClient;
using MyProject.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace MyProject.Api.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode;
            string statusMessage;
            object data = new();

            switch (exception)
            {
                case BusinessException ex:
                    statusCode = HttpStatusCode.BadRequest;
                    statusMessage = ex.Message;
                    data = ex.ErrorDetails;
                    break;

                case NotFoundException ex:
                    statusCode = HttpStatusCode.NotFound;
                    statusMessage = ex.Message;
                    break;

                case ArgumentException ex:
                    statusCode = HttpStatusCode.BadRequest;
                    statusMessage = ex.Message;
                    break;

                case SqlException ex:
                    statusCode = HttpStatusCode.InternalServerError;
                    statusMessage = "A database error occurred.";

                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    statusMessage = "An unexpected error occurred.";
                    break;
            }

            var response = new
            {
                StatusCode = statusCode,
                StatusMessage = statusMessage,
                Data = data,
                IsSuccess = false,
                TotalCount = 0
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonSerializerOptions));
        }
    }
}
