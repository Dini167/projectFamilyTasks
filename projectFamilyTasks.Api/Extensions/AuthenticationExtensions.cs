using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using projectFamilyTasks.Api.Middleware;
using projectFamilyTasks.Api.Services;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Infrastructure.Settings;

namespace projectFamilyTasks.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = WriteLoginRequiredAsync,
                    OnForbidden = context => ErrorResponseWriter.WriteAsync(
                        context.HttpContext, StatusCodes.Status403Forbidden, ErrorMessages.Forbidden)
                };
            });

        services.AddAuthorization();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }

    private static Task WriteLoginRequiredAsync(JwtBearerChallengeContext context)
    {
        context.HandleResponse();

        var message = context.AuthenticateFailure is SecurityTokenExpiredException
            ? ErrorMessages.SessionExpired
            : ErrorMessages.LoginRequired;

        return ErrorResponseWriter.WriteAsync(context.HttpContext, StatusCodes.Status401Unauthorized, message);
    }
}