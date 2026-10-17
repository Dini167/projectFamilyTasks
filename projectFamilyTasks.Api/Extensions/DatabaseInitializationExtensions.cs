using Microsoft.EntityFrameworkCore;
using projectFamilyTasks.Infrastructure.Persistence;
using projectFamilyTasks.Infrastructure.Persistence.Seed;

namespace projectFamilyTasks.Api.Extensions;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
        await scope.ServiceProvider.GetRequiredService<DbSeeder>().SeedAsync(CancellationToken.None);
    }
}
