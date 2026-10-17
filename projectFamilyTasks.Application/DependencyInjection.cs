using Microsoft.Extensions.DependencyInjection;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Application.Services;

namespace projectFamilyTasks.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(config => config.AddMaps(typeof(DependencyInjection).Assembly));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IChildService, ChildService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IHouseTaskService, HouseTaskService>();
        services.AddScoped<IRoundService, RoundService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<ICompletionService, CompletionService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<ISwapService, SwapService>();

        return services;
    }
}
