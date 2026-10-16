using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.DTOs.HouseTasks;

public class HouseTaskRequestDto
{
    public string Description { get; set; } = string.Empty;
    public int MinAge { get; set; }
    public int EstimatedMinutes { get; set; }
    public TimeOfDay TimeOfDay { get; set; }
    public Frequency Frequency { get; set; }
    public int PointsValue { get; set; }
    public List<int> CategoryIds { get; set; } = new();
}
