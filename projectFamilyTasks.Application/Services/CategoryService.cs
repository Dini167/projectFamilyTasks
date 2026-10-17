using AutoMapper;
using projectFamilyTasks.Application.DTOs.HouseTasks;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Interfaces.Services;

namespace projectFamilyTasks.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categories;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categories, IMapper mapper)
    {
        _categories = categories;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync(CancellationToken ct)
    {
        var categories = await _categories.GetAllAsync(ct);
        return _mapper.Map<List<CategoryResponseDto>>(categories);
    }
}
