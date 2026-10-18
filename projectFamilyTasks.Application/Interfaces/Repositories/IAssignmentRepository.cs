using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Interfaces.Repositories;

public interface IAssignmentRepository : IGenericRepository<Assignment>
{
    Task<PagedResultDto<Assignment>> GetAvailablePagedAsync(
        int roundId, int childAge, PagedQueryDto query, CancellationToken ct);

    Task<PagedResultDto<Assignment>> GetByChildPagedAsync(
        int roundId, int childId, PagedQueryDto query, CancellationToken ct);

    Task<int> CountByChildAsync(int roundId, int childId, CancellationToken ct);

    Task<Assignment?> GetForUpdateAsync(int id, CancellationToken ct);

    Task<bool> AnyTakenForTaskAsync(int houseTaskId, CancellationToken ct);

    Task RemoveUnassignedForTaskAsync(int houseTaskId, CancellationToken ct);
}
