using Microsoft.EntityFrameworkCore;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Domain.Entities;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Infrastructure.Persistence.Repositories;

public class SwapRequestRepository : GenericRepository<SwapRequest>, ISwapRequestRepository
{
    public SwapRequestRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedResultDto<SwapRequest>> GetPagedByStatusAsync(
        SwapStatus? status, PagedQueryDto query, CancellationToken ct)
    {
        var requests = Context.SwapRequests
            .AsNoTracking()
            .Include(s => s.RequesterAssignment).ThenInclude(a => a.HouseTask)
            .Include(s => s.RequesterAssignment).ThenInclude(a => a.Child)
            .Include(s => s.TargetAssignment).ThenInclude(a => a.HouseTask)
            .Include(s => s.TargetAssignment).ThenInclude(a => a.Child)
            .AsQueryable();

        if (status.HasValue)
            requests = requests.Where(s => s.Status == status);

        return await requests
            .OrderByDescending(s => s.CreatedAt)
            .ToPagedResultAsync(query.Page, query.PageSize, ct);
    }
}
