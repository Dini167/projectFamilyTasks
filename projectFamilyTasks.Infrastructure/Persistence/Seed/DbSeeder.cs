using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Domain.Entities;
using projectFamilyTasks.Domain.Enums;
using projectFamilyTasks.Infrastructure.Settings;

namespace projectFamilyTasks.Infrastructure.Persistence.Seed;

public class DbSeeder
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _hasher;
    private readonly SeedSettings _settings;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(
        AppDbContext context, IPasswordHasher hasher, IOptions<SeedSettings> options, ILogger<DbSeeder> logger)
    {
        _context = context;
        _hasher = hasher;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        await SeedTasksAsync(ct);
        await SeedParentAsync(ct);
        await SeedDemoChildrenAsync(ct);
    }

    private async Task SeedTasksAsync(CancellationToken ct)
    {
        if (await _context.HouseTasks.AnyAsync(ct))
            return;

        var kitchen = new Category { Name = "מטבח" };
        var cleaning = new Category { Name = "ניקיון" };
        var rides = new Category { Name = "הסעות" };
        var laundry = new Category { Name = "כביסה" };
        var tidying = new Category { Name = "סידור הבית" };
        _context.Categories.AddRange(kitchen, cleaning, rides, laundry, tidying);

        _context.HouseTasks.AddRange(
            NewTask("הכנת ארוחת ערב", 12, 45, TimeOfDay.Evening, Frequency.Daily, 10, kitchen),
            NewTask("סידור השולחן לארוחה", 5, 10, TimeOfDay.Evening, Frequency.Daily, 3, kitchen),
            NewTask("שטיפת כלים", 8, 20, TimeOfDay.Evening, Frequency.Daily, 5, kitchen),
            NewTask("הוצאת האשפה", 8, 5, TimeOfDay.Evening, Frequency.Daily, 3, cleaning),
            NewTask("הסעת הקטנים למוסדות", 13, 30, TimeOfDay.Morning, Frequency.Daily, 12, rides),
            NewTask("קיפול כביסה", 7, 20, TimeOfDay.Noon, Frequency.TwiceAWeek, 6, laundry),
            NewTask("ניקוי חדר המגורים", 10, 30, TimeOfDay.Weekly, Frequency.Weekly, 8, cleaning, tidying),
            NewTask("הכנות לשבת", 11, 40, TimeOfDay.Weekly, Frequency.Weekly, 10, tidying, kitchen));

        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Seeded 5 categories and 8 house tasks");
    }

    private async Task SeedParentAsync(CancellationToken ct)
    {
        if (await _context.ParentAccesses.AnyAsync(ct))
            return;
        if (string.IsNullOrWhiteSpace(_settings.InitialParentsPassword))
        {
            _logger.LogWarning("Seed:InitialParentsPassword is not set, so no parent was created");
            return;
        }

        _context.Parents.Add(new Parent { UserName = "parent", Name = "הורה" });
        _context.ParentAccesses.Add(new ParentAccess
        {
            PasswordHash = _hasher.Hash(_settings.InitialParentsPassword),
            MustChangePassword = true
        });

        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Seeded the first parent (the initial password must be changed on first login)");
    }

    private async Task SeedDemoChildrenAsync(CancellationToken ct)
    {
        if (await _context.Children.AnyAsync(ct))
            return;
        if (string.IsNullOrWhiteSpace(_settings.DemoPassword))
        {
            _logger.LogWarning("Seed:DemoPassword is not set, so no demo children were created");
            return;
        }

        var passwordHash = _hasher.Hash(_settings.DemoPassword);
        _context.Children.AddRange(
            NewChild("yossi", "יוסי", 6, passwordHash),
            NewChild("dana", "דנה", 9, passwordHash),
            NewChild("noa", "נועה", 13, passwordHash));

        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Seeded 3 demo children");
    }

    private static HouseTask NewTask(
        string description, int minAge, int minutes, TimeOfDay timeOfDay, Frequency frequency, int points,
        params Category[] categories)
    {
        return new HouseTask
        {
            Description = description,
            MinAge = minAge,
            EstimatedMinutes = minutes,
            TimeOfDay = timeOfDay,
            Frequency = frequency,
            PointsValue = points,
            Categories = categories.ToList()
        };
    }

    private static Child NewChild(string userName, string name, int age, string passwordHash)
    {
        return new Child
        {
            UserName = userName,
            Name = name,
            Age = age,
            PasswordHash = passwordHash,
            IsApproved = true
        };
    }
}
