using Microsoft.EntityFrameworkCore;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Infrastructure.Persistence.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct)
    {
        return await Context.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);
    }

    public async Task<List<Category>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken ct)
    {
        return await Context.Categories.Where(c => ids.Contains(c.Id)).ToListAsync(ct);
    }
}