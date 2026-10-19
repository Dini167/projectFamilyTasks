namespace projectFamilyTasks.Application.DTOs.Rounds;

public class ChildSummaryDto
{
    public int ChildId { get; init; }
    public string ChildName { get; init; } = string.Empty;
    public IReadOnlyList<AssignmentResponseDto> Assignments { get; init; } = Array.Empty<AssignmentResponseDto>();
    public int CompletionsCount { get; init; }
    public IReadOnlyList<int> CompletionsByDay { get; init; } = Array.Empty<int>();
    public int PointsEarned { get; init; }
}
