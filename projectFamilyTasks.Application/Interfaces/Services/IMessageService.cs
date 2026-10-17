using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Chat;

namespace projectFamilyTasks.Application.Interfaces.Services;

public interface IMessageService
{
    Task<MessageResponseDto> SendAsync(SendMessageRequestDto request, CancellationToken ct);

    Task<PagedResultDto<MessageResponseDto>> GetPagedAsync(PagedQueryDto query, CancellationToken ct);
}
