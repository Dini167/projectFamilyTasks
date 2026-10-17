using projectFamilyTasks.Application.DTOs.HouseTasks;

namespace projectFamilyTasks.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync(CancellationToken ct);
}
