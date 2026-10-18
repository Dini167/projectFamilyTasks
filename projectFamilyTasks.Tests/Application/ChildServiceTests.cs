using AutoMapper;
using Moq;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs.Children;
using projectFamilyTasks.Application.Exceptions;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Application.Services;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Tests.Application;

public class ChildServiceTests
{
    private readonly Mock<IFamilyMemberRepository> _members = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IPasswordHasher> _hasher = new();

    private readonly ChildService _service;

    public ChildServiceTests()
    {
        _service = new ChildService(
            _members.Object, _unitOfWork.Object, _hasher.Object, new Mock<IMapper>().Object);
    }

    [Fact]
    public async Task ResetPasswordAsync_ExistingChild_StoresTheNewHashAndSavesOnce()
    {
        var child = new Child { Id = 5, PasswordHash = "old-hash" };
        _members.Setup(m => m.GetChildForUpdateAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(child);
        _hasher.Setup(h => h.Hash("fresh-pass")).Returns("new-hash");

        await _service.ResetPasswordAsync(5, new ResetChildPasswordRequestDto { NewPassword = "fresh-pass" },
            CancellationToken.None);

        Assert.Equal("new-hash", child.PasswordHash);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_UnknownChild_ThrowsNotFoundAndSavesNothing()
    {
        _members.Setup(m => m.GetChildForUpdateAsync(9, It.IsAny<CancellationToken>())).ReturnsAsync((Child?)null);

        var error = await Assert.ThrowsAsync<NotFoundException>(() => _service.ResetPasswordAsync(
            9, new ResetChildPasswordRequestDto { NewPassword = "fresh-pass" }, CancellationToken.None));

        Assert.Equal(ErrorMessages.ChildNotFound, error.Message);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
