using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Children;

namespace projectFamilyTasks.Application.Interfaces.Services;

public interface IChildService
{
    Task<PagedResultDto<ChildResponseDto>> GetPagedAsync(PagedQueryDto query, CancellationToken ct);

    Task<ChildResponseDto> GetAsync(int id, CancellationToken ct);

    Task ApproveAsync(int id, CancellationToken ct);

    Task UpdateAsync(int id, UpdateChildRequestDto request, CancellationToken ct);

    Task DeleteAsync(int id, CancellationToken ct);
    Task ResetPasswordAsync(int id, ResetChildPasswordRequestDto request, CancellationToken ct);
}
