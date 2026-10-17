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

public class RoundService : IRoundService
{
    private readonly IRoundRepository _rounds;
    private readonly IHouseTaskRepository _tasks;
    private readonly IFamilyMemberRepository _members;
    private readonly ITaskCompletionRepository _completions;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RoundService(
        IRoundRepository rounds, IHouseTaskRepository tasks, IFamilyMemberRepository members,
        ITaskCompletionRepository completions, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _rounds = rounds;
        _tasks = tasks;
        _members = members;
        _completions = completions;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RoundResponseDto> OpenAsync(OpenRoundRequestDto request, CancellationToken ct)
    {
        if (await _rounds.GetCurrentAsync(ct) is not null)
            throw new ConflictException(ErrorMessages.RoundAlreadyOpen);
        if (request.EndDate is not null && request.EndDate <= DateTime.UtcNow)
            throw new BadRequestException(ErrorMessages.EndDateInPast);

        var taskIds = await _tasks.GetAllIdsAsync(ct);
        if (taskIds.Count == 0)
            throw new BadRequestException(ErrorMessages.NoTasksForRound);

        var round = _mapper.Map<Round>(request);
        round.StartDate = DateTime.UtcNow;
        round.Status = RoundStatus.OpenForSelection;
        round.Assignments = taskIds.Select(taskId => new Assignment { HouseTaskId = taskId }).ToList();

        await _rounds.AddAsync(round, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<RoundResponseDto>(round);
    }

    public async Task UpdateAsync(int id, UpdateRoundRequestDto request, CancellationToken ct)
    {
        var round = await _rounds.GetForUpdateAsync(id, ct) ?? throw new NotFoundException(ErrorMessages.RoundNotFound);
        if (!IsTransitionAllowed(round.Status, request.Status))
            throw new BadRequestException(ErrorMessages.RoundTransitionInvalid);

        round.Status = request.Status;
        if (request.Status == RoundStatus.Closed)
            round.EndDate = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<PagedResultDto<RoundResponseDto>> GetPagedAsync(PagedQueryDto query, CancellationToken ct)
    {
        var page = await _rounds.GetPagedAsync(query, ct);
        return _mapper.MapPaged<Round, RoundResponseDto>(page);
    }

    public async Task<RoundResponseDto> GetCurrentAsync(CancellationToken ct)
    {
        var round = await _rounds.GetCurrentAsync(ct) ?? throw new NotFoundException(ErrorMessages.NoCurrentRound);
        return _mapper.Map<RoundResponseDto>(round);
    }

    public async Task<RoundSummaryDto> GetSummaryAsync(int id, CancellationToken ct)
    {
        var round = await _rounds.GetSummaryAsync(id, ct) ?? throw new NotFoundException(ErrorMessages.RoundNotFound);
        var children = await _members.GetApprovedChildrenAsync(ct);
        var totals = (await _completions.GetTotalsByRoundAsync(id, ct)).ToDictionary(total => total.ChildId);

        var summary = _mapper.Map<RoundSummaryDto>(round);
        summary.Children = children.Select(child => BuildChildSummary(child, round, totals)).ToList();
        summary.UnassignedTasks = _mapper.Map<List<AssignmentResponseDto>>(round.Assignments.Where(a => a.ChildId is null));

        return summary;
    }

    private ChildSummaryDto BuildChildSummary(
        Child child, Round round, IReadOnlyDictionary<int, ChildCompletionTotal> totals)
    {
        totals.TryGetValue(child.Id, out var total);

        return new ChildSummaryDto
        {
            ChildId = child.Id,
            ChildName = child.Name,
            Assignments = _mapper.Map<List<AssignmentResponseDto>>(round.Assignments.Where(a => a.ChildId == child.Id)),
            CompletionsCount = total?.Count ?? 0,
            PointsEarned = total?.Points ?? 0
        };
    }

    private static bool IsTransitionAllowed(RoundStatus from, RoundStatus to)
    {
        return (from, to) is (RoundStatus.OpenForSelection, RoundStatus.Active)
            or (RoundStatus.Active, RoundStatus.Closed)
            or (RoundStatus.OpenForSelection, RoundStatus.Closed);
    }
}
