using AutoMapper;
using Moq;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs.Swaps;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Application.Services;
using projectFamilyTasks.Domain.Entities;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Tests.Application;

public class SwapServiceTests
{
    private const int RequesterId = 5;
    private const int SiblingId = 6;

    private readonly Mock<ISwapRequestRepository> _swaps = new();
    private readonly Mock<IAssignmentRepository> _assignments = new();
    private readonly Mock<IChatMessageRepository> _chat = new();
    private readonly Mock<IRoundRepository> _rounds = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();

    private readonly SwapService _service;

    public SwapServiceTests()
    {
        _service = new SwapService(
            _swaps.Object, _assignments.Object, _chat.Object, _rounds.Object,
            _currentUser.Object, _unitOfWork.Object, _mapper.Object);
    }

    private static SwapRequest SwapWithStatus(SwapStatus status)
    {
        return new SwapRequest
        {
            Id = 1,
            Status = status,
            RequesterAssignment = new Assignment { Id = 10, ChildId = RequesterId },
            TargetAssignment = new Assignment { Id = 20, ChildId = SiblingId }
        };
    }

    private void GivenDecider(string role, int userId, SwapRequest swap)
    {
        _currentUser.Setup(u => u.Role).Returns(role);
        _currentUser.Setup(u => u.UserId).Returns(userId);
        _swaps.Setup(s => s.GetForUpdateAsync(swap.Id, It.IsAny<CancellationToken>())).ReturnsAsync(swap);
    }

    [Fact]
    public async Task DecideAsync_ParentApproves_SwapsTheChildrenOfBothAssignments()
    {
        // Arrange: האח כבר אישר (ממתין להורה), וההורה מחליט לאשר.
        var swap = SwapWithStatus(SwapStatus.PendingParent);
        GivenDecider(Roles.Parent, userId: 1, swap);

        // Act: אישור ההורה.
        await _service.DecideAsync(swap.Id, new SwapDecisionRequestDto { Approve = true }, CancellationToken.None);

        // Assert: הילדים התחלפו בין שני השיבוצים, הבקשה אושרה, והשמירה נקראה פעם אחת.
        Assert.Equal(SiblingId, swap.RequesterAssignment.ChildId);
        Assert.Equal(RequesterId, swap.TargetAssignment.ChildId);
        Assert.Equal(SwapStatus.Approved, swap.Status);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DecideAsync_SiblingApproves_MovesToParentWithoutChangingAssignments()
    {
        // Arrange: הבקשה ממתינה לאח, והאח (הבעלים של המשימה היעד) מאשר.
        var swap = SwapWithStatus(SwapStatus.PendingSibling);
        GivenDecider(Roles.Child, userId: SiblingId, swap);

        // Act: אישור האח.
        await _service.DecideAsync(swap.Id, new SwapDecisionRequestDto { Approve = true }, CancellationToken.None);

        // Assert: הבקשה עברה להורה, אבל השיבוצים נשארו אצל הבעלים המקוריים.
        Assert.Equal(SwapStatus.PendingParent, swap.Status);
        Assert.Equal(RequesterId, swap.RequesterAssignment.ChildId);
        Assert.Equal(SiblingId, swap.TargetAssignment.ChildId);
    }
}
