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

        CreateMap<Round, RoundSummaryDto>();

        CreateMap<Assignment, AssignmentResponseDto>()
            .IncludeMembers(a => a.HouseTask)
            .ForMember(d => d.CompletionsInPeriod, o => o.Ignore())
            .ForMember(d => d.PeriodLimit, o => o.Ignore());
        CreateMap<HouseTask, AssignmentResponseDto>(MemberList.None);

        CreateMap<TaskCompletion, CompletionResponseDto>();
    }
}
