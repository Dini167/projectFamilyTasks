using projectFamilyTasks.Domain.Common;

namespace projectFamilyTasks.Domain.Entities;

public class TaskCompletion : BaseEntity
{
    public int AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;
    public DateTime CompletedAt { get; set; }
    public int PointsEarned { get; set; }
}
