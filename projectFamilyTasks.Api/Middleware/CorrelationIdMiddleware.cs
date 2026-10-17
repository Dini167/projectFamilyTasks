using System.Diagnostics;
using System.Text.RegularExpressions;

namespace projectFamilyTasks.Api.Middleware;

public class CorrelationIdMiddleware
{
    private static readonly Regex ValidId = new("^[A-Za-z0-9-]{1,64}$", RegexOptions.Compiled);

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startedAt = Stopwatch.GetTimestamp();
        var correlationId = ReadOrCreateId(context);

        context.Items[HttpContextKeys.CorrelationId] = correlationId;
        context.Response.Headers[HttpContextKeys.CorrelationIdHeader] = correlationId;

        context.Response.OnCompleted(() =>
        {
            LogRequest(context, correlationId, startedAt);
            return Task.CompletedTask;
        });

        using (_logger.BeginCorrelationScope(correlationId))
        {
            await _next(context);
        }
    }

    private void LogRequest(HttpContext context, string correlationId, long startedAt)
    {
        var elapsedMs = Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds;

        using (_logger.BeginCorrelationScope(correlationId))
        {
            _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs:0} ms",
                context.Request.Method, context.Request.Path.Value, context.Response.StatusCode, elapsedMs);
        }
    }

    private static string ReadOrCreateId(HttpContext context)
    {
        var fromClient = context.Request.Headers[HttpContextKeys.CorrelationIdHeader].FirstOrDefault();

        return fromClient is not null && ValidId.IsMatch(fromClient) ? fromClient : Guid.NewGuid().ToString();
    }
}
