using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using projectFamilyTasks.Api.Middleware;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Api.Extensions;

public static class RateLimitingExtensions
{
    public const string RecoveryPolicy = "recovery";

    private const int RecoveryAttempts = 5;
    private static readonly TimeSpan RecoveryWindow = TimeSpan.FromMinutes(15);

    public static IServiceCollection AddRecoveryRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter(RecoveryPolicy, window =>
            {
                window.PermitLimit = RecoveryAttempts;
                window.Window = RecoveryWindow;
                window.QueueLimit = 0;
            });
            options.OnRejected = (context, _) => new ValueTask(ErrorResponseWriter.WriteAsync(
                context.HttpContext, StatusCodes.Status429TooManyRequests, ErrorMessages.TooManyAttempts));
        });

        return services;
    }
}
