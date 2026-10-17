using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Domain.Entities;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.Interfaces.Repositories;

public interface ISwapRequestRepository : IGenericRepository<SwapRequest>
{
    Task<PagedResultDto<SwapRequest>> GetPagedByStatusAsync(
        SwapStatus? status, int? involvingChildId, PagedQueryDto query, CancellationToken ct);

    Task<SwapRequest?> GetDetailsAsync(int id, CancellationToken ct);

    Task<SwapRequest?> GetForUpdateAsync(int id, CancellationToken ct);

    Task<bool> ExistsPendingForAssignmentsAsync(int assignmentA, int assignmentB, CancellationToken ct);
}