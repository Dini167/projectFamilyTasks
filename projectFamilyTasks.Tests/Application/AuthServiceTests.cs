using AutoMapper;
using Moq;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs.Auth;
using projectFamilyTasks.Application.Exceptions;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Application.Services;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Tests.Application;

public class AuthServiceTests
{
    private readonly Mock<IParentAccessRepository> _access = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<IRecoveryKeyValidator> _recovery = new();

    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(
            new Mock<IFamilyMemberRepository>().Object, _access.Object, _unitOfWork.Object, _hasher.Object,
            _recovery.Object, new Mock<ITokenService>().Object, new Mock<ICurrentUserService>().Object,
            new Mock<IMapper>().Object);
    }

    private static ResetParentsPasswordRequestDto Request(string key)
    {
        return new ResetParentsPasswordRequestDto { RecoveryKey = key, NewPassword = "brand-new-pass" };
    }

    [Fact]
    public async Task ResetParentsPasswordAsync_ValidKey_StoresNewHashClearsForcedChangeAndSavesOnce()
    {
        var access = new ParentAccess { PasswordHash = "old", MustChangePassword = true };
        _recovery.Setup(r => r.IsValid("good-key")).Returns(true);
        _access.Setup(a => a.GetSingleAsync(It.IsAny<CancellationToken>())).ReturnsAsync(access);
        _hasher.Setup(h => h.Hash("brand-new-pass")).Returns("new-hash");

        await _service.ResetParentsPasswordAsync(Request("good-key"), CancellationToken.None);

        Assert.Equal("new-hash", access.PasswordHash);
        Assert.False(access.MustChangePassword);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetParentsPasswordAsync_WrongKey_ThrowsBadRequestAndChangesNothing()
    {
        var access = new ParentAccess { PasswordHash = "old" };
        _recovery.Setup(r => r.IsValid(It.IsAny<string?>())).Returns(false);
        _access.Setup(a => a.GetSingleAsync(It.IsAny<CancellationToken>())).ReturnsAsync(access);

        var error = await Assert.ThrowsAsync<BadRequestException>(
            () => _service.ResetParentsPasswordAsync(Request("bad-key"), CancellationToken.None));

        Assert.Equal(ErrorMessages.RecoveryKeyWrong, error.Message);
        Assert.Equal("old", access.PasswordHash);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
