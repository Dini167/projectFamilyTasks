using Microsoft.EntityFrameworkCore;
using projectFamilyTasks.Application.Common;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Infrastructure.Persistence.Repositories;

public class TaskCompletionRepository : GenericRepository<TaskCompletion>, ITaskCompletionRepository
{
    public TaskCompletionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<int> CountSinceAsync(int assignmentId, DateTime sinceUtc, CancellationToken ct)
    {
        return await Context.TaskCompletions
            .CountAsync(c => c.AssignmentId == assignmentId && c.CompletedAt >= sinceUtc, ct);
    }

    public async Task<IReadOnlyList<ChildCompletionTotal>> GetTotalsByRoundAsync(int roundId, CancellationToken ct)
    {
        return await Context.TaskCompletions
            .AsNoTracking()
            .Where(c => c.Assignment.RoundId == roundId && c.Assignment.ChildId != null)
            .GroupBy(c => c.Assignment.ChildId!.Value)
            .Select(g => new ChildCompletionTotal(g.Key, g.Count(), g.Sum(c => c.PointsEarned)))
            .ToListAsync(ct);
    }
}