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

public class AssignmentServiceTests
{
    private readonly Mock<IAssignmentRepository> _assignments = new();
    private readonly Mock<IRoundRepository> _rounds = new();
    private readonly Mock<IFamilyMemberRepository> _members = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();

    private readonly AssignmentService _service;

    public AssignmentServiceTests()
    {
        _service = new AssignmentService(
            _assignments.Object, _rounds.Object, _members.Object,
            _currentUser.Object, _unitOfWork.Object, _mapper.Object);
    }

    private static Round OpenRound(int id = 1, int quota = 2)
    {
        return new Round { Id = id, Status = RoundStatus.OpenForSelection, TasksQuotaPerChild = quota };
    }

    private static Child ChildOf(int id, int age)
    {
        return new Child { Id = id, Name = "דנה", Age = age, IsApproved = true };
    }

    private static Assignment FreeAssignment(int id, int roundId, int minAge)
    {
        return new Assignment { Id = id, RoundId = roundId, HouseTask = new HouseTask { MinAge = minAge } };
    }

    private void GivenChildTakes(Round round, Child child, Assignment assignment, int alreadyTaken)
    {
        _rounds.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(round);
        _currentUser.Setup(u => u.UserId).Returns(child.Id);
        _members.Setup(m => m.GetChildAsync(child.Id, It.IsAny<CancellationToken>())).ReturnsAsync(child);
        _assignments.Setup(a => a.GetForUpdateAsync(assignment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignment);
        _assignments.Setup(a => a.CountByChildAsync(round.Id, child.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alreadyTaken);
    }

    [Fact]
    public async Task TakeAsync_FreeSuitableAssignmentUnderQuota_AssignsChildAndSavesOnce()
    {
        // Arrange: ילדה בת 9, משימה מגיל 8, ועוד לא נתפסה אף משימה בסבב עם מכסה של 2.
        var round = OpenRound(quota: 2);
        var child = ChildOf(id: 5, age: 9);
        var assignment = FreeAssignment(id: 10, roundId: round.Id, minAge: 8);
        GivenChildTakes(round, child, assignment, alreadyTaken: 0);

        // Act: הילדה תופסת את המשימה.
        await _service.TakeAsync(assignment.Id, CancellationToken.None);

        // Assert: השיבוץ שייך לה, נרשם מתי נתפס, והשמירה נקראה בדיוק פעם אחת.
        Assert.Equal(child.Id, assignment.ChildId);
        Assert.NotNull(assignment.AssignedAt);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TakeAsync_AssignmentAlreadyTaken_ThrowsConflictAndDoesNotSave()
    {
        // Arrange: המשימה כבר שייכת לילד אחר (מספר 99), והילדה (מספר 5) מנסה לתפוס אותה.
        var round = OpenRound();
        var child = ChildOf(id: 5, age: 9);
        var assignment = FreeAssignment(id: 10, roundId: round.Id, minAge: 8);
        assignment.ChildId = 99;
        GivenChildTakes(round, child, assignment, alreadyTaken: 0);

        // Act: הניסיון לתפוס, שאמור להיכשל.
        var error = await Assert.ThrowsAsync<ConflictException>(
            () => _service.TakeAsync(assignment.Id, CancellationToken.None));

        // Assert: קוד 409 עם ההודעה הנכונה בעברית, השיבוץ לא השתנה, ושום דבר לא נשמר.
        Assert.Equal(409, error.StatusCode);
        Assert.Equal(ErrorMessages.AssignmentTaken, error.Message);
        Assert.Equal(99, assignment.ChildId);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task TakeAsync_ChildReachedQuota_ThrowsBadRequestAndDoesNotSave()
    {
        // Arrange: מכסה של 2, והילדה כבר תפסה 2 (alreadyTaken), והשיבוץ החדש פנוי ומתאים לגילה.
        var round = OpenRound(quota: 2);
        var child = ChildOf(id: 5, age: 9);
        var assignment = FreeAssignment(id: 10, roundId: round.Id, minAge: 8);
        GivenChildTakes(round, child, assignment, alreadyTaken: 2);

        // Act: הניסיון לתפוס משימה שלישית, שאמור להיכשל.
        var error = await Assert.ThrowsAsync<BadRequestException>(
            () => _service.TakeAsync(assignment.Id, CancellationToken.None));

        // Assert: קוד 400 עם הודעת המכסה בעברית, השיבוץ נשאר פנוי, ושום דבר לא נשמר.
        Assert.Equal(400, error.StatusCode);
        Assert.Equal(ErrorMessages.QuotaReached, error.Message);
        Assert.Null(assignment.ChildId);
        Assert.Null(assignment.AssignedAt);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
