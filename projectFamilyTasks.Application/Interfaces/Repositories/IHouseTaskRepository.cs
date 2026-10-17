using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.HouseTasks;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Interfaces.Repositories;

public interface IHouseTaskRepository : IGenericRepository<HouseTask>
{
    Task<PagedResultDto<HouseTask>> GetPagedAsync(HouseTaskQueryDto query, CancellationToken ct);

    Task<HouseTask?> GetForUpdateAsync(int id, CancellationToken ct);

    Task<HouseTask?> GetByIdWithCategoriesAsync(int id, CancellationToken ct);

    Task<List<int>> GetAllIdsAsync(CancellationToken ct);
}
