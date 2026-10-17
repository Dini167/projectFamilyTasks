namespace projectFamilyTasks.Application.DTOs.Rounds;

public class RoundSummaryDto : RoundResponseDto
{
    public IReadOnlyList<ChildSummaryDto> Children { get; set; } = Array.Empty<ChildSummaryDto>();
    public IReadOnlyList<AssignmentResponseDto> UnassignedTasks { get; set; } =
        Array.Empty<AssignmentResponseDto>();
}
