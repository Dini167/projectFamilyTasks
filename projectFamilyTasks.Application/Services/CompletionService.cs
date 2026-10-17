using AutoMapper;
using projectFamilyTasks.Application.Common;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs.Rounds;
using projectFamilyTasks.Application.Exceptions;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Domain.Entities;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.Services;

public class CompletionService : ICompletionService
{
    private readonly IAssignmentRepository _assignments;
    private readonly IRoundRepository _rounds;
    private readonly IFamilyMemberRepository _members;
    private readonly ITaskCompletionRepository _completions;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CompletionService(
        IAssignmentRepository assignments, IRoundRepository rounds, IFamilyMemberRepository members,
        ITaskCompletionRepository completions, ICurrentUserService currentUser, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _assignments = assignments;
        _rounds = rounds;
        _members = members;
        _completions = completions;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CompletionResponseDto> CompleteAsync(int assignmentId, CancellationToken ct)
    {
        var assignment = await _assignments.GetForUpdateAsync(assignmentId, ct)
                         ?? throw new NotFoundException(ErrorMessages.AssignmentNotFound);
        if (assignment.ChildId != _currentUser.UserId)
            throw new ForbiddenException(ErrorMessages.NotYourAssignment);

        var round = await _rounds.GetCurrentAsync(ct);
        if (round is null || round.Id != assignment.RoundId || round.Status != RoundStatus.Active)
            throw new BadRequestException(ErrorMessages.RoundNotActive);

        var now = DateTime.UtcNow;
        var (limit, since) = FrequencyRules.GetPeriod(assignment.HouseTask.Frequency, now);
        if (await _completions.CountSinceAsync(assignment.Id, since, ct) >= limit)
            throw new ConflictException(ErrorMessages.AlreadyCompleted);

        var child = await _members.GetChildForUpdateAsync(_currentUser.UserId, ct)
                    ?? throw new NotFoundException(ErrorMessages.ChildNotFound);
        var completion = new TaskCompletion
        {
            AssignmentId = assignment.Id,
            CompletedAt = now,
            PointsEarned = assignment.HouseTask.PointsValue
        };

        await _completions.AddAsync(completion, ct);
        child.PointsBalance += completion.PointsEarned;
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<CompletionResponseDto>(completion);
    }
}
