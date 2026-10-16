using Microsoft.EntityFrameworkCore;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Infrastructure.Persistence.Repositories;

public class FamilyMemberRepository : GenericRepository<FamilyMember>, IFamilyMemberRepository
{
    public FamilyMemberRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<FamilyMember?> GetByUserNameAsync(string userName, CancellationToken ct)
    {
        return await Context.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.UserName == userName, ct);
    }

    public async Task<PagedResultDto<Child>> GetChildrenPagedAsync(
        PagedQueryDto query, CancellationToken ct)
    {
        return await Context.Children
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Id)
            .ToPagedResultAsync(query.Page, query.PageSize, ct);
    }
}
