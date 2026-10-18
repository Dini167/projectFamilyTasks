using Microsoft.EntityFrameworkCore;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.HouseTasks;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Infrastructure.Persistence.Repositories;

public class HouseTaskRepository : GenericRepository<HouseTask>, IHouseTaskRepository
{
    public HouseTaskRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedResultDto<HouseTask>> GetPagedAsync(HouseTaskQueryDto query, CancellationToken ct)
    {
        var tasks = Context.HouseTasks.AsNoTracking().Include(t => t.Categories).AsQueryable();

        if (query.TimeOfDay.HasValue)
            tasks = tasks.Where(t => t.TimeOfDay == query.TimeOfDay);
        if (query.CategoryId.HasValue)
            tasks = tasks.Where(t => t.Categories.Any(c => c.Id == query.CategoryId));
        if (query.MaxMinAge.HasValue)
            tasks = tasks.Where(t => t.MinAge <= query.MaxMinAge);

        return await Sort(tasks, query).ToPagedResultAsync(query.Page, query.PageSize, ct);
    }

    private static IOrderedQueryable<HouseTask> Sort(IQueryable<HouseTask> tasks, PagedQueryDto query)
    {
        return query.SortBy?.ToLowerInvariant() switch
        {
            "points" => tasks.OrderByDirection(t => t.PointsValue, query.Descending),
            "minage" => tasks.OrderByDirection(t => t.MinAge, query.Descending),
            "minutes" => tasks.OrderByDirection(t => t.EstimatedMinutes, query.Descending),
            _ => tasks.OrderByDirection(t => t.Description, query.Descending)
        };
    }

    public async Task<HouseTask?> GetForUpdateAsync(int id, CancellationToken ct)
    {
        return await Context.HouseTasks.Include(t => t.Categories).FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<HouseTask?> GetByIdWithCategoriesAsync(int id, CancellationToken ct)
    {
        return await Context.HouseTasks.AsNoTracking()
            .Include(t => t.Categories)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<List<int>> GetAllIdsAsync(CancellationToken ct)
    {
        return await Context.HouseTasks.AsNoTracking().Select(t => t.Id).ToListAsync(ct);
    }
}
