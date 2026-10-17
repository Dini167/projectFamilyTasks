namespace projectFamilyTasks.Api.Middleware;

public static class CorrelationExtensions
{
    public static string GetCorrelationId(this HttpContext context)
    {
        return context.Items[HttpContextKeys.CorrelationId]?.ToString() ?? context.TraceIdentifier;
    }

    public static IDisposable? BeginCorrelationScope(this ILogger logger, string correlationId)
    {
        return logger.BeginScope(new Dictionary<string, object> { [HttpContextKeys.CorrelationId] = correlationId });
    }
}
