using AutoMapper;
using projectFamilyTasks.Application.DTOs.HouseTasks;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Mapping;

public class HouseTaskProfile : Profile
{
    public HouseTaskProfile()
    {
        CreateMap<Category, CategoryResponseDto>();

        CreateMap<HouseTask, HouseTaskResponseDto>();

        CreateMap<HouseTaskRequestDto, HouseTask>();
    }
}
