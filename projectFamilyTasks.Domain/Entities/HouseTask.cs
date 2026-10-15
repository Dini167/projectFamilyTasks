using projectFamilyTasks.Domain.Common;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Domain.Entities;

public class HouseTask : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public int MinAge { get; set; }
    public int EstimatedMinutes { get; set; }
    public TimeOfDay TimeOfDay { get; set; }
    public Frequency Frequency { get; set; }
    public int PointsValue { get; set; }
    public List<Category> Categories { get; set; } = new();
}
