using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Rounds;

namespace projectFamilyTasks.Application.Interfaces.Services;

public interface IRoundService
{
    Task<RoundResponseDto> OpenAsync(OpenRoundRequestDto request, CancellationToken ct);

    Task UpdateAsync(int id, UpdateRoundRequestDto request, CancellationToken ct);

    Task<PagedResultDto<RoundResponseDto>> GetPagedAsync(PagedQueryDto query, CancellationToken ct);

    Task<RoundResponseDto> GetCurrentAsync(CancellationToken ct);

    Task<RoundSummaryDto> GetSummaryAsync(int id, CancellationToken ct);
}
