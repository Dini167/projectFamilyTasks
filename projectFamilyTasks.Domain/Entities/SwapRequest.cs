using projectFamilyTasks.Domain.Common;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Domain.Entities;

public class SwapRequest : BaseEntity
{
    public int RequesterAssignmentId { get; set; }
    public Assignment RequesterAssignment { get; set; } = null!;
    public int TargetAssignmentId { get; set; }
    public Assignment TargetAssignment { get; set; } = null!;
    public SwapStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
