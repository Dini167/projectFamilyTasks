using Microsoft.EntityFrameworkCore;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Infrastructure.Persistence.Repositories;

public class ParentAccessRepository : GenericRepository<ParentAccess>, IParentAccessRepository
{
    public ParentAccessRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ParentAccess?> GetSingleAsync(CancellationToken ct)
    {
        return await Context.ParentAccesses.FirstOrDefaultAsync(ct);
    }
}