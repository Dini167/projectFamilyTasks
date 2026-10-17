using Microsoft.EntityFrameworkCore;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Infrastructure.Persistence.Repositories;

public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository
{
    public AssignmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedResultDto<Assignment>> GetAvailablePagedAsync(
        int roundId, int childAge, PagedQueryDto query, CancellationToken ct)
    {
        var available = Context.Assignments
            .Where(a => a.RoundId == roundId && a.ChildId == null && a.HouseTask.MinAge <= childAge);

        return await ToPagedResultAsync(available, query, ct);
    }

    public async Task<PagedResultDto<Assignment>> GetByChildPagedAsync(
        int roundId, int childId, PagedQueryDto query, CancellationToken ct)
    {
        var mine = Context.Assignments.Where(a => a.RoundId == roundId && a.ChildId == childId);

        return await ToPagedResultAsync(mine, query, ct);
    }

    public async Task<int> CountByChildAsync(int roundId, int childId, CancellationToken ct)
    {
        return await Context.Assignments
            .CountAsync(a => a.RoundId == roundId && a.ChildId == childId, ct);
    }

    public async Task<Assignment?> GetForUpdateAsync(int id, CancellationToken ct)
    {
        return await Context.Assignments
            .Include(a => a.HouseTask)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    private static async Task<PagedResultDto<Assignment>> ToPagedResultAsync(
        IQueryable<Assignment> assignments, PagedQueryDto query, CancellationToken ct)
    {
        return await assignments
            .AsNoTracking()
            .Include(a => a.HouseTask)
            .OrderBy(a => a.HouseTask.Description)
            .ThenBy(a => a.Id)
            .ToPagedResultAsync(query.Page, query.PageSize, ct);
    }

    public async Task<bool> AnyForTaskAsync(int houseTaskId, CancellationToken ct)
    {
        return await Context.Assignments.AnyAsync(a => a.HouseTaskId == houseTaskId, ct);
    }
}
