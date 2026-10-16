using Microsoft.EntityFrameworkCore;
using projectFamilyTasks.Application.DTOs;

namespace projectFamilyTasks.Infrastructure.Persistence;

public static class PagingExtensions
{
    public static async Task<PagedResultDto<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query, int page, int pageSize, CancellationToken ct)
    {
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResultDto<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
