using Microsoft.EntityFrameworkCore;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Domain.Common;

namespace projectFamilyTasks.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Context;

    public GenericRepository(AppDbContext context)
    {
        Context = context;
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await Context.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task AddAsync(T entity, CancellationToken ct)
    {
        await Context.Set<T>().AddAsync(entity, ct);
    }

    public void Update(T entity)
    {
        Context.Set<T>().Update(entity);
    }

    public void Remove(T entity)
    {
        Context.Set<T>().Remove(entity);
    }
}
