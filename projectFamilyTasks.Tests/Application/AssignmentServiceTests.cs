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

    [Fact]
    public async Task TakeAsync_UnitOfWorkReportsConflict_ConflictPropagatesToCaller()
    {
        // Arrange: תרחיש תקין לגמרי, אבל השמירה "נכשלת" כי ילד אחר שמר ראשון.
        var round = OpenRound(quota: 2);
        var child = ChildOf(id: 5, age: 9);
        var assignment = FreeAssignment(id: 10, roundId: round.Id, minAge: 8);
        GivenChildTakes(round, child, assignment, alreadyTaken: 0);
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConflictException(ErrorMessages.ConcurrencyConflict));

        // Act: הילדה מנסה לתפוס, והשמירה זורקת התנגשות.
        var error = await Assert.ThrowsAsync<ConflictException>(
            () => _service.TakeAsync(assignment.Id, CancellationToken.None));

        // Assert: השגיאה עברה הלאה כמות שהיא (409 וההודעה בעברית), והשירות אכן ניסה לשמור פעם אחת.
        Assert.Equal(409, error.StatusCode);
        Assert.Equal(ErrorMessages.ConcurrencyConflict, error.Message);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveFromRoundAsync_FreeAssignmentInCurrentRound_RemovesItAndSavesOnce()
    {
        // Arrange: סבב נוכחי ושיבוץ פנוי שלא נתפס על ידי אף ילד.
        var round = OpenRound();
        var assignment = FreeAssignment(id: 10, roundId: round.Id, minAge: 8);
        _rounds.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(round);
        _assignments.Setup(a => a.GetForUpdateAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(assignment);

        // Act: ההורה מסיר את המשימה מהסבב.
        await _service.RemoveFromRoundAsync(round.Id, assignment.Id, CancellationToken.None);

        // Assert: השיבוץ נמחק ונשמר פעם אחת.
        _assignments.Verify(a => a.Remove(assignment), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveFromRoundAsync_AssignmentAlreadyTaken_ThrowsConflictAndRemovesNothing()
    {
        // Arrange: ילדה כבר תפסה את המשימה.
        var round = OpenRound();
        var assignment = FreeAssignment(id: 10, roundId: round.Id, minAge: 8);
        assignment.ChildId = 5;
        _rounds.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(round);
        _assignments.Setup(a => a.GetForUpdateAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(assignment);

        // Act: ההורה מנסה להסיר משימה שנתפסה.
        var error = await Assert.ThrowsAsync<ConflictException>(
            () => _service.RemoveFromRoundAsync(round.Id, assignment.Id, CancellationToken.None));

        // Assert: 409 עם הודעה בעברית, ושום דבר לא נמחק ולא נשמר.
        Assert.Equal(ErrorMessages.AssignmentTaken, error.Message);
        _assignments.Verify(a => a.Remove(It.IsAny<Assignment>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RemoveFromRoundAsync_AssignmentOfAnotherRound_ThrowsNotFound()
    {
        // Arrange: השיבוץ שייך לסבב אחר מהסבב הנוכחי.
        var round = OpenRound(id: 2);
        var assignment = FreeAssignment(id: 10, roundId: 1, minAge: 8);
        _rounds.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(round);
        _assignments.Setup(a => a.GetForUpdateAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(assignment);

        // Act and Assert: 404 עם הודעה בעברית, ושום דבר לא נמחק.
        var error = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.RemoveFromRoundAsync(round.Id, assignment.Id, CancellationToken.None));
        Assert.Equal(ErrorMessages.AssignmentNotFound, error.Message);
        _assignments.Verify(a => a.Remove(It.IsAny<Assignment>()), Times.Never);
    }
}
