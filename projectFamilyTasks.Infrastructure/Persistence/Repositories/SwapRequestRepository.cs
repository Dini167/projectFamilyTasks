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
        SwapStatus? status, int? involvingChildId, PagedQueryDto query, CancellationToken ct)
    {
        var requests = WithDetails(Context.SwapRequests.AsNoTracking());

        if (status.HasValue)
            requests = requests.Where(s => s.Status == status);
        if (involvingChildId.HasValue)
            requests = requests.Where(s => s.RequesterAssignment.ChildId == involvingChildId
                                           || s.TargetAssignment.ChildId == involvingChildId);

        return await requests
            .OrderByDescending(s => s.CreatedAt)
            .ToPagedResultAsync(query.Page, query.PageSize, ct);
    }

    public async Task<SwapRequest?> GetDetailsAsync(int id, CancellationToken ct)
    {
        return await WithDetails(Context.SwapRequests.AsNoTracking()).FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<SwapRequest?> GetForUpdateAsync(int id, CancellationToken ct)
    {
        return await Context.SwapRequests
            .Include(s => s.RequesterAssignment)
            .Include(s => s.TargetAssignment)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<bool> ExistsPendingForAssignmentsAsync(int assignmentA, int assignmentB, CancellationToken ct)
    {
        return await Context.SwapRequests.AnyAsync(s =>
            (s.Status == SwapStatus.PendingSibling || s.Status == SwapStatus.PendingParent)
            && (s.RequesterAssignmentId == assignmentA || s.RequesterAssignmentId == assignmentB
                || s.TargetAssignmentId == assignmentA || s.TargetAssignmentId == assignmentB), ct);
    }

    private static IQueryable<SwapRequest> WithDetails(IQueryable<SwapRequest> requests)
    {
        return requests
            .Include(s => s.RequesterAssignment).ThenInclude(a => a.HouseTask)
            .Include(s => s.RequesterAssignment).ThenInclude(a => a.Child)
            .Include(s => s.TargetAssignment).ThenInclude(a => a.HouseTask)
            .Include(s => s.TargetAssignment).ThenInclude(a => a.Child);
    }
}