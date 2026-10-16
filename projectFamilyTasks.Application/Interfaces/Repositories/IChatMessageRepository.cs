using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Interfaces.Repositories;

public interface IChatMessageRepository : IGenericRepository<ChatMessage>
{
    Task<PagedResultDto<ChatMessage>> GetPagedAsync(PagedQueryDto query, CancellationToken ct);
}
