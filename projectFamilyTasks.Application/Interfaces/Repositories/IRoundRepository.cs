using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Interfaces.Repositories;

public interface IRoundRepository : IGenericRepository<Round>
{
    Task<Round?> GetCurrentAsync(CancellationToken ct);

    Task<Round?> GetSummaryAsync(int roundId, CancellationToken ct);

    Task<PagedResultDto<Round>> GetPagedAsync(PagedQueryDto query, CancellationToken ct);
}
