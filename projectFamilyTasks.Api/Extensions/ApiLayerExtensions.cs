using Microsoft.AspNetCore.Mvc;
using projectFamilyTasks.Api.Middleware;
using projectFamilyTasks.Application.DTOs;

namespace projectFamilyTasks.Api.Extensions;

public static class ApiLayerExtensions
{
    public static IServiceCollection AddApiLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
                options.InvalidModelStateResponseFactory = BuildValidationResponse);

        services.AddSwaggerWithJwt();
        services.AddCorsFromConfiguration(configuration);

        return services;
    }

    private static void AddCorsFromConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options => options.AddDefaultPolicy(policy => policy
            .WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders(HttpContextKeys.CorrelationIdHeader)));
    }

    private static IActionResult BuildValidationResponse(ActionContext context)
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors.Select(error => $"{entry.Key}: {error.ErrorMessage}"))
            .ToList();

        var response = new ErrorResponseDto
        {
            Status = StatusCodes.Status400BadRequest,
            Message = "הנתונים שנשלחו אינם תקינים.",
            CorrelationId = context.HttpContext.GetCorrelationId(),
            Errors = errors
        };

        return new BadRequestObjectResult(response);
    }
}
