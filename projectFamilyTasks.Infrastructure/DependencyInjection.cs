using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Infrastructure.Persistence;
using projectFamilyTasks.Infrastructure.Persistence.Repositories;

namespace projectFamilyTasks.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IFamilyMemberRepository, FamilyMemberRepository>();
        services.AddScoped<IHouseTaskRepository, HouseTaskRepository>();
        services.AddScoped<IRoundRepository, RoundRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<ISwapRequestRepository, SwapRequestRepository>();
        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();

        return services;
    }
}
