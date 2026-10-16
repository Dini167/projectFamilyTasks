using projectFamilyTasks.Application.DTOs.Rounds;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.DTOs.Swaps;

public class SwapRequestResponseDto
{
    public int Id { get; init; }
    public AssignmentResponseDto RequesterAssignment { get; init; } = null!;
    public AssignmentResponseDto TargetAssignment { get; init; } = null!;
    public SwapStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}
