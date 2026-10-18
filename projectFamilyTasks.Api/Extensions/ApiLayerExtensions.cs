using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using projectFamilyTasks.Api.Middleware;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs;

namespace projectFamilyTasks.Api.Extensions;

public static class ApiLayerExtensions
{
    private static readonly Regex HasHebrew = new(@"\p{IsHebrew}", RegexOptions.Compiled);

    public static IServiceCollection AddApiLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options =>
            {
                HebrewBindingMessages.Apply(options.ModelBindingMessageProvider);
                options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
            })
            .ConfigureApiBehaviorOptions(options =>
                options.InvalidModelStateResponseFactory = BuildValidationResponse);

        services.AddSwaggerWithJwt();
        services.AddCorsFromConfiguration(configuration);
        services.AddRecoveryRateLimiting();

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
        var errors = context.ModelState.Values
            .SelectMany(entry => entry.Errors)
            .Select(error => HasHebrew.IsMatch(error.ErrorMessage) ? error.ErrorMessage : ErrorMessages.InvalidFormat)
            .Distinct()
            .ToList();

        var response = new ErrorResponseDto
        {
            Status = StatusCodes.Status400BadRequest,
            Message = ErrorMessages.InvalidData,
            CorrelationId = context.HttpContext.GetCorrelationId(),
            Errors = errors
        };

        return new BadRequestObjectResult(response);
    }
}