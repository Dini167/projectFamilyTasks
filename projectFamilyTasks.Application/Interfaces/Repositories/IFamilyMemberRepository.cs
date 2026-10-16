using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Interfaces.Repositories;

public interface IFamilyMemberRepository : IGenericRepository<FamilyMember>
{
    Task<FamilyMember?> GetByUserNameAsync(string userName, CancellationToken ct);

    Task<PagedResultDto<Child>> GetChildrenPagedAsync(PagedQueryDto query, CancellationToken ct);
}
