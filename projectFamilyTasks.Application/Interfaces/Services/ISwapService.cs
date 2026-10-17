using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Swaps;

namespace projectFamilyTasks.Application.Interfaces.Services;

public interface ISwapService
{
    Task<SwapRequestResponseDto> CreateAsync(CreateSwapRequestDto request, CancellationToken ct);

    Task DecideAsync(int id, SwapDecisionRequestDto request, CancellationToken ct);

    Task<PagedResultDto<SwapRequestResponseDto>> GetPagedAsync(SwapQueryDto query, CancellationToken ct);
}
