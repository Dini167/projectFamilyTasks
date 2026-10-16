using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.DTOs.Rounds;

public class AssignmentResponseDto
{
    public int Id { get; init; }
    public int RoundId { get; init; }
    public int HouseTaskId { get; init; }
    public string Description { get; init; } = string.Empty;
    public int EstimatedMinutes { get; init; }
    public TimeOfDay TimeOfDay { get; init; }
    public Frequency Frequency { get; init; }
    public int PointsValue { get; init; }
    public int? ChildId { get; init; }
    public string? ChildName { get; init; }
    public DateTime? AssignedAt { get; init; }
}
