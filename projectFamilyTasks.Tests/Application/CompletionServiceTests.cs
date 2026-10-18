using AutoMapper;
using Moq;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.Exceptions;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Application.Services;
using projectFamilyTasks.Domain.Entities;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Tests.Application;

public class CompletionServiceTests
{
    private readonly Mock<IAssignmentRepository> _assignments = new();
    private readonly Mock<IRoundRepository> _rounds = new();
    private readonly Mock<IFamilyMemberRepository> _members = new();
    private readonly Mock<ITaskCompletionRepository> _completions = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();

    private readonly CompletionService _service;

    public CompletionServiceTests()
    {
        _service = new CompletionService(
            _assignments.Object, _rounds.Object, _members.Object, _completions.Object,
            _currentUser.Object, _unitOfWork.Object, _mapper.Object);
    }

    [Fact]
    public async Task CompleteAsync_DailyTaskAlreadyCompletedToday_ThrowsConflictAndAddsNoPoints()
    {
        // Arrange: משימה יומית של ילד 5 בסבב פעיל, שכבר סומנה פעם אחת היום (CountSinceAsync מחזיר 1).
        var child = new Child { Id = 5, Name = "דנה", PointsBalance = 10 };
        var round = new Round { Id = 1, Status = RoundStatus.Active };
        var assignment = new Assignment
        {
            Id = 10, RoundId = round.Id, ChildId = child.Id,
            HouseTask = new HouseTask { Frequency = Frequency.Daily, PointsValue = 3 }
        };
        _currentUser.Setup(u => u.UserId).Returns(child.Id);
        _rounds.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(round);
        _assignments.Setup(a => a.GetForUpdateAsync(assignment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignment);
        _completions.Setup(c => c.CountSinceAsync(assignment.Id, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act: הסימון השני באותו יום, שאמור להיכשל.
        var error = await Assert.ThrowsAsync<ConflictException>(
            () => _service.CompleteAsync(assignment.Id, CancellationToken.None));

        // Assert: קוד 409 עם ההודעה בעברית, הנקודות לא השתנו, שום דבר לא נוסף ולא נשמר.
        Assert.Equal(409, error.StatusCode);
        Assert.Equal(ErrorMessages.AlreadyCompleted, error.Message);
        Assert.Equal(10, child.PointsBalance);
        _completions.Verify(
            c => c.AddAsync(It.IsAny<TaskCompletion>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
