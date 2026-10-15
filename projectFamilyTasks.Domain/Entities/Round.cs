using projectFamilyTasks.Domain.Common;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Domain.Entities;

public class Round : BaseEntity
{
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public RoundStatus Status { get; set; }
    public int TasksQuotaPerChild { get; set; }
    public List<Assignment> Assignments { get; set; } = new();
}
