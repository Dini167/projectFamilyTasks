using AutoMapper;
using projectFamilyTasks.Application.Common;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Chat;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Services;

public class MessageService : IMessageService
{
    private readonly IChatMessageRepository _messages;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MessageService(
        IChatMessageRepository messages, ICurrentUserService currentUser, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _messages = messages;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MessageResponseDto> SendAsync(SendMessageRequestDto request, CancellationToken ct)
    {
        var message = _mapper.Map<ChatMessage>(request);
        message.SenderId = _currentUser.UserId;
        message.SentAt = DateTime.UtcNow;

        await _messages.AddAsync(message, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var saved = await _messages.GetByIdWithSenderAsync(message.Id, ct);
        return _mapper.Map<MessageResponseDto>(saved);
    }

    public async Task<PagedResultDto<MessageResponseDto>> GetPagedAsync(PagedQueryDto query, CancellationToken ct)
    {
        var page = await _messages.GetPagedAsync(query, ct);
        return _mapper.MapPaged<ChatMessage, MessageResponseDto>(page);
    }
}
