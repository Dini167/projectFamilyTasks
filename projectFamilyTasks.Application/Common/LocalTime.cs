namespace projectFamilyTasks.Application.Common;

public static class LocalTime
{
    private static readonly TimeZoneInfo Israel = FindIsraelZone();

    public static DateTime StartOfDayUtc(DateTime utcNow)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(utcNow, Israel);
        return TimeZoneInfo.ConvertTimeToUtc(local.Date, Israel);
    }

    public static DateTime StartOfWeekUtc(DateTime utcNow)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(utcNow, Israel);
        var sunday = local.Date.AddDays(-(int)local.DayOfWeek);
        return TimeZoneInfo.ConvertTimeToUtc(sunday, Israel);
    }

    public static int DayOfWeek(DateTime utc)
    {
        return (int)TimeZoneInfo.ConvertTimeFromUtc(utc, Israel).DayOfWeek;
    }

    private static TimeZoneInfo FindIsraelZone()
    {
        foreach (var id in new[] { "Asia/Jerusalem", "Israel Standard Time" })
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
            catch (TimeZoneNotFoundException) { }
        }

        return TimeZoneInfo.Utc;
    }
}