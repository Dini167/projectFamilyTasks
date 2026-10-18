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
            NewTask("הכנות לשבת", 11, 40, TimeOfDay.Weekly, Frequency.Weekly, 10, tidying, kitchen),
            NewTask("סידור המיטה", 4, 5, TimeOfDay.Morning, Frequency.Daily, 2, tidying),
            NewTask("סידור הצעצועים והמשחקים", 4, 10, TimeOfDay.Evening, Frequency.Daily, 3, tidying),
            NewTask("סידור הנעליים בכניסה", 5, 5, TimeOfDay.Evening, Frequency.Daily, 2, tidying),
            NewTask("פינוי הכלים מהשולחן", 6, 10, TimeOfDay.Evening, Frequency.Daily, 3, kitchen),
            NewTask("השקיית הצמחים", 6, 10, TimeOfDay.Morning, Frequency.TwiceAWeek, 3, tidying),
            NewTask("סידור הבגדים בארונות", 7, 15, TimeOfDay.Weekly, Frequency.Weekly, 4, laundry, tidying),
            NewTask("ייבוש וסידור הכלים בארון", 7, 10, TimeOfDay.Evening, Frequency.Daily, 3, kitchen),
            NewTask("טאטוא הרצפה", 8, 15, TimeOfDay.Evening, Frequency.Daily, 4, cleaning),
            NewTask("הכנת כריכים לבית הספר", 9, 15, TimeOfDay.Morning, Frequency.Daily, 4, kitchen),
            NewTask("תליית כביסה", 9, 20, TimeOfDay.Morning, Frequency.TwiceAWeek, 5, laundry),
            NewTask("ניקוי אבק", 9, 20, TimeOfDay.Weekly, Frequency.Weekly, 5, cleaning),
            NewTask("הכנת שולחן שבת", 10, 30, TimeOfDay.Weekly, Frequency.Weekly, 8, tidying, kitchen),
            NewTask("הפעלת מכונת כביסה", 11, 10, TimeOfDay.Noon, Frequency.TwiceAWeek, 4, laundry),
            NewTask("הוצאת פחי מחזור", 11, 10, TimeOfDay.Weekly, Frequency.Weekly, 3, cleaning),
            NewTask("שטיפת הרצפה", 12, 25, TimeOfDay.Weekly, Frequency.Weekly, 7, cleaning),
            NewTask("ניקוי שירותים ואמבטיה", 13, 25, TimeOfDay.Weekly, Frequency.Weekly, 8, cleaning));

        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Seeded 5 categories and 24 house tasks");
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
}
