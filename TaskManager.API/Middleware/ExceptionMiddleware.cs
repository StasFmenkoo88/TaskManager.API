using System.Net;
using System.Text.Json;
using TaskManager.API.Exceptions;

namespace TaskManager.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(
                    ex,
                    "Unhandled exception while processing {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = ex.Message,
                };

                if (ex is NotFoundException)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                }
                else if (ex is ConflictException)
                {
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                }
                else if (ex is UnauthorizedException)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}