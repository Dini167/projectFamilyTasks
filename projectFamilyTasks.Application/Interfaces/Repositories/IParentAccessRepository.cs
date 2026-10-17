using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Interfaces.Repositories;

public interface IParentAccessRepository : IGenericRepository<ParentAccess>
{
    Task<ParentAccess?> GetSingleAsync(CancellationToken ct);
}