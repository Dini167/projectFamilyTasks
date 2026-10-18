using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Common;

public static class RoundRules
{
    public const int MaxQuotaMultiplier = 2;

    public static int MaxTasksPerChild(Round round)
    {
        return round.TasksQuotaPerChild * MaxQuotaMultiplier;
    }

    // כמה משימות הילד חייב לבחור לפחות: המכסה, או פחות אם אין מספיק משימות שמתאימות לגילו בסבב.
    public static int RequiredTasks(Round round, Child child)
    {
        var reachable = round.Assignments.Count(
            a => a.ChildId == child.Id || (a.ChildId is null && a.HouseTask.MinAge <= child.Age));

        return Math.Min(round.TasksQuotaPerChild, reachable);
    }
}
