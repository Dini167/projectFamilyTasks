using projectFamilyTasks.Domain.Common;

namespace projectFamilyTasks.Domain.Entities;

public class Assignment : BaseEntity, IHasVersion
{
    public int RoundId { get; set; }
    public Round Round { get; set; } = null!;
    public int HouseTaskId { get; set; }
    public HouseTask HouseTask { get; set; } = null!;
    public int? ChildId { get; set; }
    public Child? Child { get; set; }
    public DateTime? AssignedAt { get; set; }

    public Guid Version { get; set; }
}
