using AutoMapper;
using projectFamilyTasks.Application.Common;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.HouseTasks;
using projectFamilyTasks.Application.Exceptions;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Services;

public class HouseTaskService : IHouseTaskService
{
    private readonly IHouseTaskRepository _tasks;
    private readonly ICategoryRepository _categories;
    private readonly IAssignmentRepository _assignments;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HouseTaskService(
        IHouseTaskRepository tasks, ICategoryRepository categories, IAssignmentRepository assignments,
        IUnitOfWork unitOfWork, IMapper mapper)
    {
        _tasks = tasks;
        _categories = categories;
        _assignments = assignments;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<HouseTaskResponseDto>> GetPagedAsync(HouseTaskQueryDto query, CancellationToken ct)
    {
        var page = await _tasks.GetPagedAsync(query, ct);
        return _mapper.MapPaged<HouseTask, HouseTaskResponseDto>(page);
    }

    public async Task<HouseTaskResponseDto> GetAsync(int id, CancellationToken ct)
    {
        var task = await _tasks.GetByIdWithCategoriesAsync(id, ct)
                   ?? throw new NotFoundException(ErrorMessages.TaskNotFound);
        return _mapper.Map<HouseTaskResponseDto>(task);
    }

    public async Task<HouseTaskResponseDto> CreateAsync(HouseTaskRequestDto request, CancellationToken ct)
    {
        var task = _mapper.Map<HouseTask>(request);
        task.Categories = await ResolveCategoriesAsync(request.CategoryIds, ct);

        await _tasks.AddAsync(task, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<HouseTaskResponseDto>(task);
    }

    public async Task UpdateAsync(int id, HouseTaskRequestDto request, CancellationToken ct)
    {
        var task = await _tasks.GetForUpdateAsync(id, ct) ?? throw new NotFoundException(ErrorMessages.TaskNotFound);

        _mapper.Map(request, task);
        task.Categories.Clear();
        task.Categories.AddRange(await ResolveCategoriesAsync(request.CategoryIds, ct));

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var task = await _tasks.GetByIdAsync(id, ct) ?? throw new NotFoundException(ErrorMessages.TaskNotFound);
        if (await _assignments.AnyForTaskAsync(id, ct))
            throw new ConflictException(ErrorMessages.TaskHasAssignments);

        _tasks.Remove(task);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<List<Category>> ResolveCategoriesAsync(List<int> categoryIds, CancellationToken ct)
    {
        var ids = categoryIds.Distinct().ToList();
        var categories = await _categories.GetByIdsAsync(ids, ct);

        return categories.Count == ids.Count
            ? categories
            : throw new BadRequestException(ErrorMessages.CategoryNotFound);
    }
}
