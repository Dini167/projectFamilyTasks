using projectFamilyTasks.Application.DTOs.Rounds;

namespace projectFamilyTasks.Application.Common;

public static class CompletionProgress
{
    public const int DaysInWeek = 7;

    public static void Apply(
        IEnumerable<AssignmentResponseDto> items, IReadOnlyCollection<CompletionStamp> stamps, DateTime utcNow)
    {
        foreach (var item in items)
        {
            var (limit, since) = FrequencyRules.GetPeriod(item.Frequency, utcNow);
            item.PeriodLimit = limit;
            item.CompletionsInPeriod = stamps.Count(s => s.AssignmentId == item.Id && s.CompletedAt >= since);
        }
    }

    public static IReadOnlyList<int> CountByDayOfWeek(IEnumerable<CompletionStamp> stamps)
    {
        var days = new int[DaysInWeek];
        foreach (var stamp in stamps)
            days[LocalTime.DayOfWeek(stamp.CompletedAt)]++;

        return days;
    }
}
