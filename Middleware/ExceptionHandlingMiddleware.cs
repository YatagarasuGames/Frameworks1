using Frameworks1.DTOs;
using Frameworks1.Errors;
using Frameworks1.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Frameworks1.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestId = RequestIdService.GetOrCreate(context);

            try
            {
                await _next(context);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Ошибка предметной области. requestId={RequestId}", requestId);
                await WriteError(context, ex.StatusCode, ex.Code, ex.Message, requestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка. requestId={RequestId}", requestId);
                await WriteError(context, 500, "internal_error", "Внутренняя ошибка сервера", requestId);
            }
        }

        private static async Task WriteError(HttpContext context, int statusCode, string code, string message, string requestId)
        {
            if (context.Response.HasStarted)
                return;

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";

            var payload = new ErrorResponse(code, message, requestId);
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
