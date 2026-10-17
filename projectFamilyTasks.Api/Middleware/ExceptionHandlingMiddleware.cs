using projectFamilyTasks.Application.Common.Constants;
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
            using (_logger.BeginCorrelationScope(context.GetCorrelationId()))
            {
                _logger.LogInformation("Handled error {StatusCode}: {Message}", ex.StatusCode, ex.Message);
            }

            await ErrorResponseWriter.WriteAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            using (_logger.BeginCorrelationScope(context.GetCorrelationId()))
            {
                _logger.LogError(ex, "Unhandled exception");
            }

            await ErrorResponseWriter.WriteAsync(
                context, StatusCodes.Status500InternalServerError, ErrorMessages.Unexpected);
        }
    }
}