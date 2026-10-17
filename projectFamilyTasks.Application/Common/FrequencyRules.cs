using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.Common;

public static class FrequencyRules
{
    public static (int Limit, DateTime SinceUtc) GetPeriod(Frequency frequency, DateTime utcNow)
    {
        return frequency switch
        {
            Frequency.Daily => (1, LocalTime.StartOfDayUtc(utcNow)),
            Frequency.TwiceAWeek => (2, LocalTime.StartOfWeekUtc(utcNow)),
            _ => (1, LocalTime.StartOfWeekUtc(utcNow))
        };
    }
}