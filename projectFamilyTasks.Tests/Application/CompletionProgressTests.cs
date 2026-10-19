using projectFamilyTasks.Application.Common;
using projectFamilyTasks.Application.DTOs.Rounds;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Tests.Application;

public class CompletionProgressTests
{
    [Fact]
    public void Apply_CountsOnlyCompletionsInsideThePeriodOfTheFrequency()
    {
        // Arrange: משימה יומית ומשימה של פעמיים בשבוע. לכל אחת ביצוע אחד עכשיו וביצוע ישן מלפני 20 יום.
        var now = DateTime.UtcNow;
        var daily = new AssignmentResponseDto { Id = 1, Frequency = Frequency.Daily };
        var twice = new AssignmentResponseDto { Id = 2, Frequency = Frequency.TwiceAWeek };
        var stamps = new[]
        {
            new CompletionStamp(1, now), new CompletionStamp(1, now.AddDays(-20)),
            new CompletionStamp(2, now), new CompletionStamp(2, now.AddDays(-20))
        };

        // Act
        CompletionProgress.Apply(new[] { daily, twice }, stamps, now);

        // Assert: רק הביצוע של עכשיו נספר, והמגבלה לפי התדירות.
        Assert.Equal(1, daily.CompletionsInPeriod);
        Assert.Equal(1, daily.PeriodLimit);
        Assert.Equal(1, twice.CompletionsInPeriod);
        Assert.Equal(2, twice.PeriodLimit);
    }

    [Fact]
    public void CountByDayOfWeek_PlacesEachCompletionOnItsDayStartingFromSunday()
    {
        // Arrange: 18.10.2026 הוא יום ראשון, ו-21.10.2026 הוא יום רביעי.
        var stamps = new[]
        {
            new CompletionStamp(1, new DateTime(2026, 10, 18, 10, 0, 0, DateTimeKind.Utc)),
            new CompletionStamp(1, new DateTime(2026, 10, 21, 10, 0, 0, DateTimeKind.Utc)),
            new CompletionStamp(2, new DateTime(2026, 10, 21, 11, 0, 0, DateTimeKind.Utc))
        };

        // Act
        var days = CompletionProgress.CountByDayOfWeek(stamps);

        // Assert
        Assert.Equal(new[] { 1, 0, 0, 2, 0, 0, 0 }, days);
    }
}
