using projectFamilyTasks.Application.DTOs.Rounds;

namespace projectFamilyTasks.Application.Interfaces.Services;

public interface ICompletionService
{
    Task<CompletionResponseDto> CompleteAsync(int assignmentId, CancellationToken ct);
}
