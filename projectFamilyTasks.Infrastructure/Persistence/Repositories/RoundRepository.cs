using Microsoft.EntityFrameworkCore;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Domain.Entities;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Infrastructure.Persistence.Repositories;

public class RoundRepository : GenericRepository<Round>, IRoundRepository
{
    public RoundRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Round?> GetCurrentAsync(CancellationToken ct)
    {
        return await Context.Rounds
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Status != RoundStatus.Closed, ct);
    }

    public async Task<Round?> GetSummaryAsync(int roundId, CancellationToken ct)
    {
        return await Context.Rounds
            .AsNoTracking()
            .Include(r => r.Assignments).ThenInclude(a => a.Child)
            .Include(r => r.Assignments).ThenInclude(a => a.HouseTask)
            .FirstOrDefaultAsync(r => r.Id == roundId, ct);
    }

    public async Task<PagedResultDto<Round>> GetPagedAsync(PagedQueryDto query, CancellationToken ct)
    {
        return await Context.Rounds
            .AsNoTracking()
            .OrderByDescending(r => r.StartDate)
            .ToPagedResultAsync(query.Page, query.PageSize, ct);
    }
}
