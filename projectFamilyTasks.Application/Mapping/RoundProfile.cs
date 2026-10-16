using AutoMapper;
using projectFamilyTasks.Application.DTOs.Rounds;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Mapping;

public class RoundProfile : Profile
{
    public RoundProfile()
    {
        CreateMap<OpenRoundRequestDto, Round>();

        CreateMap<Round, RoundResponseDto>();

        CreateMap<Assignment, AssignmentResponseDto>().IncludeMembers(a => a.HouseTask);
        CreateMap<HouseTask, AssignmentResponseDto>(MemberList.None);

        CreateMap<TaskCompletion, CompletionResponseDto>();
    }
}
