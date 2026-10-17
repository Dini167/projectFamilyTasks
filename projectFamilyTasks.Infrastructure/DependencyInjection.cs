using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Infrastructure.Persistence;
using projectFamilyTasks.Infrastructure.Persistence.Repositories;
using projectFamilyTasks.Infrastructure.Security;
using projectFamilyTasks.Infrastructure.Settings;

namespace projectFamilyTasks.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        services.AddSingleton<ITokenService, JwtTokenService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IFamilyMemberRepository, FamilyMemberRepository>();
        services.AddScoped<IHouseTaskRepository, HouseTaskRepository>();
        services.AddScoped<IRoundRepository, RoundRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<ISwapRequestRepository, SwapRequestRepository>();
        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
        services.AddScoped<IParentAccessRepository, ParentAccessRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITaskCompletionRepository, TaskCompletionRepository>();

        return services;
    }
}
