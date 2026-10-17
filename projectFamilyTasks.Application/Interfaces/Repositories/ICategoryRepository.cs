using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Interfaces.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct);

    Task<List<Category>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken ct);
}