namespace projectFamilyTasks.Application.DTOs.Rounds;

public class RoundSummaryDto : RoundResponseDto
{
    public IReadOnlyList<ChildSummaryDto> Children { get; init; } = Array.Empty<ChildSummaryDto>();
    public IReadOnlyList<AssignmentResponseDto> UnassignedTasks { get; init; } =
        Array.Empty<AssignmentResponseDto>();
}
