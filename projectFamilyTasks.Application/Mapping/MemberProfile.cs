using AutoMapper;
using projectFamilyTasks.Application.DTOs.Auth;
using projectFamilyTasks.Application.DTOs.Children;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Mapping;

public class MemberProfile : Profile
{
    public MemberProfile()
    {
        CreateMap<Child, ChildResponseDto>();

        CreateMap<RegisterChildRequestDto, Child>();

        CreateMap<UpdateChildRequestDto, Child>();
    }
}
