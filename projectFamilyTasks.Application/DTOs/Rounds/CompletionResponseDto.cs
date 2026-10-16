namespace projectFamilyTasks.Application.DTOs.Rounds;

public class CompletionResponseDto
{
    public int Id { get; init; }
    public int AssignmentId { get; init; }
    public DateTime CompletedAt { get; init; }
    public int PointsEarned { get; init; }
}
