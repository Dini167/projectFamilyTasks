using AutoMapper;
using Moq;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.Exceptions;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Services;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Tests.Application;

public class HouseTaskServiceTests
{
    private readonly Mock<IHouseTaskRepository> _tasks = new();
    private readonly Mock<IAssignmentRepository> _assignments = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly HouseTaskService _service;

    public HouseTaskServiceTests()
    {
        _service = new HouseTaskService(
            _tasks.Object, new Mock<ICategoryRepository>().Object, _assignments.Object,
            _unitOfWork.Object, new Mock<IMapper>().Object);
    }

    private HouseTask GivenExistingTask(int id, bool takenByChild)
    {
        var task = new HouseTask { Id = id };
        _tasks.Setup(t => t.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        _assignments.Setup(a => a.AnyTakenForTaskAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(takenByChild);
        return task;
    }

    [Fact]
    public async Task DeleteAsync_TaskNobodyTook_RemovesUnassignedAssignmentsAndTaskThenSaves()
    {
        var task = GivenExistingTask(5, takenByChild: false);

        await _service.DeleteAsync(5, CancellationToken.None);

        _assignments.Verify(a => a.RemoveUnassignedForTaskAsync(5, It.IsAny<CancellationToken>()), Times.Once);
        _tasks.Verify(t => t.Remove(task), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_TaskTakenByChild_ThrowsConflictAndDeletesNothing()
    {
        GivenExistingTask(5, takenByChild: true);

        var error = await Assert.ThrowsAsync<ConflictException>(() => _service.DeleteAsync(5, CancellationToken.None));

        Assert.Equal(ErrorMessages.TaskHasAssignments, error.Message);
        _tasks.Verify(t => t.Remove(It.IsAny<HouseTask>()), Times.Never);
        _assignments.Verify(
            a => a.RemoveUnassignedForTaskAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
