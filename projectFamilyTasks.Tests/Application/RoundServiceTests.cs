using AutoMapper;
using Moq;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs.Rounds;
using projectFamilyTasks.Application.Exceptions;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Services;
using projectFamilyTasks.Domain.Entities;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Tests.Application;

public class RoundServiceTests
{
    private const int RoundId = 1;

    private readonly Mock<IRoundRepository> _rounds = new();
    private readonly Mock<IFamilyMemberRepository> _members = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly RoundService _service;

    public RoundServiceTests()
    {
        _service = new RoundService(
            _rounds.Object, new Mock<IHouseTaskRepository>().Object, _members.Object,
            new Mock<ITaskCompletionRepository>().Object, _unitOfWork.Object, new Mock<IMapper>().Object);
    }

    private static Assignment TaskForChild(int id, int? childId, int minAge = 4)
    {
        return new Assignment
        {
            Id = id, RoundId = RoundId, ChildId = childId, HouseTask = new HouseTask { MinAge = minAge }
        };
    }

    private Round GivenOpenRound(int quota, IReadOnlyList<Child> children, params Assignment[] assignments)
    {
        var round = new Round { Id = RoundId, Status = RoundStatus.OpenForSelection, TasksQuotaPerChild = quota };
        round.Assignments.AddRange(assignments);
        _rounds.Setup(r => r.GetForUpdateAsync(RoundId, It.IsAny<CancellationToken>())).ReturnsAsync(round);
        _rounds.Setup(r => r.GetSummaryAsync(RoundId, It.IsAny<CancellationToken>())).ReturnsAsync(round);
        _members.Setup(m => m.GetApprovedChildrenAsync(It.IsAny<CancellationToken>())).ReturnsAsync(children);
        return round;
    }

    private Task Activate()
    {
        return _service.UpdateAsync(
            RoundId, new UpdateRoundRequestDto { Status = RoundStatus.Active }, CancellationToken.None);
    }

    [Fact]
    public async Task UpdateAsync_ActivatingWhileAChildIsBelowQuota_ThrowsAndNamesTheChild()
    {
        var dana = new Child { Id = 5, Name = "דנה", Age = 9 };
        var round = GivenOpenRound(3, new[] { dana },
            TaskForChild(1, 5), TaskForChild(2, null), TaskForChild(3, null), TaskForChild(4, null));

        var error = await Assert.ThrowsAsync<BadRequestException>(Activate);

        Assert.Contains("דנה (1 מתוך 3)", error.Message);
        Assert.StartsWith(ErrorMessages.ChildrenBelowQuota, error.Message);
        Assert.Equal(RoundStatus.OpenForSelection, round.Status);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ActivatingWhenEveryChildReachedTheQuota_ActivatesAndSaves()
    {
        var dana = new Child { Id = 5, Name = "דנה", Age = 9 };
        var round = GivenOpenRound(2, new[] { dana },
            TaskForChild(1, 5), TaskForChild(2, 5), TaskForChild(3, null));

        await Activate();

        Assert.Equal(RoundStatus.Active, round.Status);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ChildHasFewerSuitableTasksThanTheQuota_OnlyThoseAreRequired()
    {
        var yossi = new Child { Id = 6, Name = "יוסי", Age = 6 };
        var round = GivenOpenRound(3, new[] { yossi },
            TaskForChild(1, 6, minAge: 5), TaskForChild(2, null, minAge: 12), TaskForChild(3, null, minAge: 13));

        await Activate();

        Assert.Equal(RoundStatus.Active, round.Status);
    }
}
