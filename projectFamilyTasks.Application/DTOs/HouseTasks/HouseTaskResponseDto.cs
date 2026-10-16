using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.DTOs.HouseTasks;

public class HouseTaskResponseDto
{
    public int Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public int MinAge { get; init; }
    public int EstimatedMinutes { get; init; }
    public TimeOfDay TimeOfDay { get; init; }
    public Frequency Frequency { get; init; }
    public int PointsValue { get; init; }
    public IReadOnlyList<CategoryResponseDto> Categories { get; init; } = Array.Empty<CategoryResponseDto>();
}
