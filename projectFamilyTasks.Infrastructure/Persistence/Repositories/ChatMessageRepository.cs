using Microsoft.EntityFrameworkCore;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Infrastructure.Persistence.Repositories;

public class ChatMessageRepository : GenericRepository<ChatMessage>, IChatMessageRepository
{
    public ChatMessageRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedResultDto<ChatMessage>> GetPagedAsync(PagedQueryDto query, CancellationToken ct)
    {
        return await Context.ChatMessages
            .AsNoTracking()
            .Include(m => m.Sender)
            .OrderByDescending(m => m.SentAt)
            .ToPagedResultAsync(query.Page, query.PageSize, ct);
    }
}
