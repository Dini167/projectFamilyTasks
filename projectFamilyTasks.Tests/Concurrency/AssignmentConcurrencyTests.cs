using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using projectFamilyTasks.Domain.Entities;
using projectFamilyTasks.Domain.Enums;
using projectFamilyTasks.Infrastructure.Persistence;

namespace projectFamilyTasks.Tests.Concurrency;

public class AssignmentConcurrencyTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    public AssignmentConcurrencyTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;

        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    private AppDbContext CreateContext()
    {
        return new AppDbContext(_options);
    }

    [Fact]
    public async Task SaveChangesAsync_TwoContextsTakeSameAssignment_SecondOneThrowsConcurrencyException()
    {
        // Arrange: סבב עם משימה אחת פנויה, ושני ילדים שרוצים אותה.
        var (assignmentId, firstChildId, secondChildId) = await SeedAssignmentAndTwoChildrenAsync();

        // Act: שני "ילדים" (שני DbContext) קוראים את אותו שיבוץ, כשהוא עדיין פנוי, ושניהם מציבים את עצמם בו.
        await using var firstContext = CreateContext();
        await using var secondContext = CreateContext();
        var seenByFirst = await firstContext.Assignments.SingleAsync(a => a.Id == assignmentId);
        var seenBySecond = await secondContext.Assignments.SingleAsync(a => a.Id == assignmentId);
        seenByFirst.ChildId = firstChildId;
        seenBySecond.ChildId = secondChildId;

        await firstContext.SaveChangesAsync();
        var save = () => secondContext.SaveChangesAsync();

        // Assert: הראשון הצליח. השני נדחה בשגיאת תחרות ספציפית, ובבסיס הנתונים נשאר הילד הראשון.
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(save);

        await using var verification = CreateContext();
        var stored = await verification.Assignments.SingleAsync(a => a.Id == assignmentId);
        Assert.Equal(firstChildId, stored.ChildId);
    }

    private async Task<(int AssignmentId, int FirstChildId, int SecondChildId)> SeedAssignmentAndTwoChildrenAsync()
    {
        await using var context = CreateContext();

        var round = new Round
        {
            StartDate = DateTime.UtcNow, Status = RoundStatus.OpenForSelection, TasksQuotaPerChild = 1
        };
        var task = new HouseTask { Description = "שטיפת כלים", MinAge = 5, EstimatedMinutes = 10, PointsValue = 5 };
        var first = new Child { UserName = "first", Name = "ראשון", Age = 9, PasswordHash = "x", IsApproved = true };
        var second = new Child { UserName = "second", Name = "שני", Age = 9, PasswordHash = "x", IsApproved = true };
        context.AddRange(round, task, first, second);
        await context.SaveChangesAsync();

        var assignment = new Assignment { RoundId = round.Id, HouseTaskId = task.Id };
        context.Add(assignment);
        await context.SaveChangesAsync();

        return (assignment.Id, first.Id, second.Id);
    }
}
