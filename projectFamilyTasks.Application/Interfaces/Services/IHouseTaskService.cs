using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.HouseTasks;

namespace projectFamilyTasks.Application.Interfaces.Services;

public interface IHouseTaskService
{
    Task<PagedResultDto<HouseTaskResponseDto>> GetPagedAsync(HouseTaskQueryDto query, CancellationToken ct);

    Task<HouseTaskResponseDto> GetAsync(int id, CancellationToken ct);

    Task<HouseTaskResponseDto> CreateAsync(HouseTaskRequestDto request, CancellationToken ct);

    Task UpdateAsync(int id, HouseTaskRequestDto request, CancellationToken ct);

    Task DeleteAsync(int id, CancellationToken ct);
}
