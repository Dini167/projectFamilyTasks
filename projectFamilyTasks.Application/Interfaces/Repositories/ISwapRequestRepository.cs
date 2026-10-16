using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Domain.Entities;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.Interfaces.Repositories;

public interface ISwapRequestRepository : IGenericRepository<SwapRequest>
{
    Task<PagedResultDto<SwapRequest>> GetPagedByStatusAsync(
        SwapStatus? status, PagedQueryDto query, CancellationToken ct);
}
