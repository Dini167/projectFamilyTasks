using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Rounds;

namespace projectFamilyTasks.Application.Interfaces.Services;

public interface IAssignmentService
{
    Task<PagedResultDto<AssignmentResponseDto>> GetAvailableAsync(PagedQueryDto query, CancellationToken ct);

    Task<PagedResultDto<AssignmentResponseDto>> GetMineAsync(PagedQueryDto query, CancellationToken ct);
    Task<PagedResultDto<AssignmentResponseDto>> GetSiblingsAsync(PagedQueryDto query, CancellationToken ct);

    Task TakeAsync(int assignmentId, CancellationToken ct);

    Task AssignToChildAsync(int assignmentId, int childId, CancellationToken ct);
    Task RemoveFromRoundAsync(int roundId, int assignmentId, CancellationToken ct);
}
