using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Interfaces.Repositories;

public interface IHouseTaskRepository : IGenericRepository<HouseTask>
{
    Task<PagedResultDto<HouseTask>> GetPagedAsync(HouseTaskQueryDto query, CancellationToken ct);
}
