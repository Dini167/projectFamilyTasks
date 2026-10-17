using AutoMapper;
using Moq;
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
}
