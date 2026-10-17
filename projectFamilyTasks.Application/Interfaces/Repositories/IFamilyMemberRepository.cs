using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Interfaces.Repositories;

public interface IFamilyMemberRepository : IGenericRepository<FamilyMember>
{
    Task<FamilyMember?> GetByUserNameAsync(string userName, CancellationToken ct);

    Task<PagedResultDto<Child>> GetChildrenPagedAsync(PagedQueryDto query, CancellationToken ct);

    Task<Child?> GetChildAsync(int id, CancellationToken ct);

    Task<Child?> GetChildForUpdateAsync(int id, CancellationToken ct);

    Task<IReadOnlyList<Child>> GetApprovedChildrenAsync(CancellationToken ct);
}
