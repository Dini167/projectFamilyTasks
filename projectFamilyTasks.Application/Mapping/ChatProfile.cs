using AutoMapper;
using projectFamilyTasks.Application.DTOs.Chat;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Mapping;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<ChatMessage, MessageResponseDto>();

        CreateMap<SendMessageRequestDto, ChatMessage>();
    }
}
