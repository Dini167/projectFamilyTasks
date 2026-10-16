using AutoMapper;
using projectFamilyTasks.Application.DTOs.Swaps;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Mapping;

public class SwapProfile : Profile
{
    public SwapProfile()
    {
        CreateMap<SwapRequest, SwapRequestResponseDto>();

        CreateMap<CreateSwapRequestDto, SwapRequest>();
    }
}
