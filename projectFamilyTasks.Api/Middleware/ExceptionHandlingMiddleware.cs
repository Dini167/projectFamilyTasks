using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.Exceptions;

namespace projectFamilyTasks.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogDebug("The client cancelled the request.");
        }
        catch (AppException ex)
        {
            _logger.LogInformation("Handled error {StatusCode}: {Message}", ex.StatusCode, ex.Message);
            await WriteErrorAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError,
                "אירעה שגיאה בלתי צפויה. יש לנסות שוב מאוחר יותר.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        var response = new ErrorResponseDto
        {
            Status = statusCode,
            Message = message,
            CorrelationId = context.Items[HttpContextKeys.CorrelationId]?.ToString() ?? context.TraceIdentifier
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}
