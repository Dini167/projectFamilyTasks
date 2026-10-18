using AutoMapper;
using projectFamilyTasks.Application.Common;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Rounds;
using projectFamilyTasks.Application.Exceptions;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Domain.Entities;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignments;
    private readonly IRoundRepository _rounds;
    private readonly IFamilyMemberRepository _members;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AssignmentService(
        IAssignmentRepository assignments, IRoundRepository rounds, IFamilyMemberRepository members,
        ICurrentUserService currentUser, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _assignments = assignments;
        _rounds = rounds;
        _members = members;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<AssignmentResponseDto>> GetAvailableAsync(PagedQueryDto query, CancellationToken ct)
    {
        var round = await _rounds.GetCurrentAsync(ct);
        if (round is null || round.Status != RoundStatus.OpenForSelection)
            return EmptyPage(query);

        var child = await GetCurrentChildAsync(ct);
        var page = await _assignments.GetAvailablePagedAsync(round.Id, child.Age, query, ct);
        return _mapper.MapPaged<Assignment, AssignmentResponseDto>(page);
    }

    public async Task<PagedResultDto<AssignmentResponseDto>> GetMineAsync(PagedQueryDto query, CancellationToken ct)
    {
        var round = await _rounds.GetCurrentAsync(ct);
        if (round is null)
            return EmptyPage(query);

        var page = await _assignments.GetByChildPagedAsync(round.Id, _currentUser.UserId, query, ct);
        return _mapper.MapPaged<Assignment, AssignmentResponseDto>(page);
    }

    public async Task TakeAsync(int assignmentId, CancellationToken ct)
    {
        var round = await _rounds.GetCurrentAsync(ct) ?? throw new BadRequestException(ErrorMessages.NoOpenSelection);
        if (round.Status != RoundStatus.OpenForSelection)
            throw new BadRequestException(ErrorMessages.SelectionClosed);

        await AssignAsync(assignmentId, await GetCurrentChildAsync(ct), round, ct);
    }

    public async Task AssignToChildAsync(int assignmentId, int childId, CancellationToken ct)
    {
        var round = await _rounds.GetCurrentAsync(ct) ?? throw new BadRequestException(ErrorMessages.NoCurrentRound);
        var child = await _members.GetChildAsync(childId, ct) ?? throw new NotFoundException(ErrorMessages.ChildNotFound);
        if (!child.IsApproved)
            throw new BadRequestException(ErrorMessages.ChildNotApproved);

        await AssignAsync(assignmentId, child, round, ct);
    }

    private async Task AssignAsync(int assignmentId, Child child, Round round, CancellationToken ct)
    {
        var assignment = await _assignments.GetForUpdateAsync(assignmentId, ct);
        if (assignment is null || assignment.RoundId != round.Id)
            throw new NotFoundException(ErrorMessages.AssignmentNotFound);
        if (assignment.ChildId is not null)
            throw new ConflictException(ErrorMessages.AssignmentTaken);
        if (assignment.HouseTask.MinAge > child.Age)
            throw new BadRequestException(ErrorMessages.TaskNotSuitableForAge);
        if (await _assignments.CountByChildAsync(round.Id, child.Id, ct) >= RoundRules.MaxTasksPerChild(round))
            throw new BadRequestException(ErrorMessages.QuotaReached);

        assignment.ChildId = child.Id;
        assignment.AssignedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RemoveFromRoundAsync(int roundId, int assignmentId, CancellationToken ct)
    {
        var round = await _rounds.GetCurrentAsync(ct) ?? throw new BadRequestException(ErrorMessages.NoCurrentRound);
        var assignment = await _assignments.GetForUpdateAsync(assignmentId, ct);
        if (assignment is null || round.Id != roundId || assignment.RoundId != roundId)
            throw new NotFoundException(ErrorMessages.AssignmentNotFound);
        if (assignment.ChildId is not null)
            throw new ConflictException(ErrorMessages.AssignmentTaken);

        _assignments.Remove(assignment);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<Child> GetCurrentChildAsync(CancellationToken ct)
    {
        return await _members.GetChildAsync(_currentUser.UserId, ct)
               ?? throw new NotFoundException(ErrorMessages.ChildNotFound);
    }

    private static PagedResultDto<AssignmentResponseDto> EmptyPage(PagedQueryDto query)
    {
        return new PagedResultDto<AssignmentResponseDto> { Page = query.Page, PageSize = query.PageSize };
    }
}
