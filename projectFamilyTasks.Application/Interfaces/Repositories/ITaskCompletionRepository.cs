using projectFamilyTasks.Application.Common;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Interfaces.Repositories;

public interface ITaskCompletionRepository : IGenericRepository<TaskCompletion>
{
    Task<int> CountSinceAsync(int assignmentId, DateTime sinceUtc, CancellationToken ct);

    Task<IReadOnlyList<ChildCompletionTotal>> GetTotalsByRoundAsync(int roundId, CancellationToken ct);
}