using AutoMapper;
using projectFamilyTasks.Application.Common;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Swaps;
using projectFamilyTasks.Application.Exceptions;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Domain.Entities;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.Services;

public class SwapService : ISwapService
{
    private readonly ISwapRequestRepository _swaps;
    private readonly IAssignmentRepository _assignments;
    private readonly IChatMessageRepository _chat;
    private readonly IRoundRepository _rounds;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SwapService(
        ISwapRequestRepository swaps, IAssignmentRepository assignments, IChatMessageRepository chat,
        IRoundRepository rounds, ICurrentUserService currentUser, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _swaps = swaps;
        _assignments = assignments;
        _chat = chat;
        _rounds = rounds;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SwapRequestResponseDto> CreateAsync(CreateSwapRequestDto request, CancellationToken ct)
    {
        var round = await _rounds.GetCurrentAsync(ct);
        if (round is null || round.Status != RoundStatus.Active)
            throw new BadRequestException(ErrorMessages.RoundNotActive);

        var mine = await GetAssignmentInRoundAsync(request.RequesterAssignmentId, round, ct);
        var target = await GetAssignmentInRoundAsync(request.TargetAssignmentId, round, ct);

        if (mine.ChildId != _currentUser.UserId)
            throw new ForbiddenException(ErrorMessages.SwapOwnAssignmentRequired);
        if (target.ChildId is null || target.ChildId == _currentUser.UserId)
            throw new BadRequestException(ErrorMessages.SwapTargetInvalid);
        if (await _swaps.ExistsPendingForAssignmentsAsync(mine.Id, target.Id, ct))
            throw new ConflictException(ErrorMessages.SwapAlreadyPending);

        var now = DateTime.UtcNow;
        var swap = _mapper.Map<SwapRequest>(request);
        swap.Status = SwapStatus.PendingSibling;
        swap.CreatedAt = now;

        await _swaps.AddAsync(swap, ct);
        await _chat.AddAsync(new ChatMessage
        {
            SenderId = _currentUser.UserId,
            SwapRequest = swap,
            SentAt = now,
            Content = string.Format(ErrorMessages.SwapChatText, mine.HouseTask.Description, target.HouseTask.Description)
        }, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<SwapRequestResponseDto>(await _swaps.GetDetailsAsync(swap.Id, ct));
    }

    public async Task DecideAsync(int id, SwapDecisionRequestDto request, CancellationToken ct)
    {
        var swap = await _swaps.GetForUpdateAsync(id, ct) ?? throw new NotFoundException(ErrorMessages.SwapNotFound);

        switch (swap.Status)
        {
            case SwapStatus.PendingSibling:
                DecideBySibling(swap, request.Approve);
                break;
            case SwapStatus.PendingParent:
                DecideByParent(swap, request.Approve);
                break;
            default:
                throw new ConflictException(ErrorMessages.SwapAlreadyClosed);
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<PagedResultDto<SwapRequestResponseDto>> GetPagedAsync(SwapQueryDto query, CancellationToken ct)
    {
        int? childId = _currentUser.Role == Roles.Child ? _currentUser.UserId : null;
        var page = await _swaps.GetPagedByStatusAsync(query.Status, childId, query, ct);
        return _mapper.MapPaged<SwapRequest, SwapRequestResponseDto>(page);
    }

    private void DecideBySibling(SwapRequest swap, bool approve)
    {
        if (_currentUser.Role != Roles.Child || swap.TargetAssignment.ChildId != _currentUser.UserId)
            throw new ForbiddenException(ErrorMessages.SwapNotYourDecision);

        swap.Status = approve ? SwapStatus.PendingParent : SwapStatus.RejectedBySibling;
    }

    private void DecideByParent(SwapRequest swap, bool approve)
    {
        if (_currentUser.Role != Roles.Parent)
            throw new ForbiddenException(ErrorMessages.SwapParentDecisionOnly);

        if (!approve)
        {
            swap.Status = SwapStatus.RejectedByParent;
            return;
        }

        var first = swap.RequesterAssignment;
        var second = swap.TargetAssignment;
        if (first.ChildId is null || second.ChildId is null || first.ChildId == second.ChildId)
            throw new ConflictException(ErrorMessages.SwapAssignmentsChanged);

        (first.ChildId, second.ChildId) = (second.ChildId, first.ChildId);
        first.AssignedAt = second.AssignedAt = DateTime.UtcNow;
        swap.Status = SwapStatus.Approved;
    }

    private async Task<Assignment> GetAssignmentInRoundAsync(int assignmentId, Round round, CancellationToken ct)
    {
        var assignment = await _assignments.GetForUpdateAsync(assignmentId, ct);

        return assignment is not null && assignment.RoundId == round.Id
            ? assignment
            : throw new NotFoundException(ErrorMessages.AssignmentNotFound);
    }
}
