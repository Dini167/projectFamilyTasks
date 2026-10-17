using projectFamilyTasks.Application.DTOs;

namespace projectFamilyTasks.Api.Middleware;

public static class ErrorResponseWriter
{
    public static async Task WriteAsync(
        HttpContext context, int statusCode, string message, IReadOnlyList<string>? errors = null)
    {
        var response = new ErrorResponseDto
        {
            Status = statusCode,
            Message = message,
            CorrelationId = context.GetCorrelationId(),
            Errors = errors
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}