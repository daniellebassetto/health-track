using HealthTrack.Core.Exceptions;

namespace HealthTrack.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
            HandleException(context, ex);
        }
    }

    private static void HandleException(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = exception switch
        {
            NotFoundException => 404,
            ArgumentException => 400,
            UnauthorizedAccessException => 401,
            _ => 500
        };
    }
}