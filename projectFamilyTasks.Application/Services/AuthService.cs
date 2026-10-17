using AutoMapper;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs.Auth;
using projectFamilyTasks.Application.DTOs.Children;
using projectFamilyTasks.Application.Exceptions;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Services;

public class AuthService : IAuthService
{
    private readonly IFamilyMemberRepository _members;
    private readonly IParentAccessRepository _access;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public AuthService(
        IFamilyMemberRepository members, IParentAccessRepository access, IUnitOfWork unitOfWork,
        IPasswordHasher hasher, ITokenService tokens, ICurrentUserService currentUser, IMapper mapper)
    {
        _members = members;
        _access = access;
        _unitOfWork = unitOfWork;
        _hasher = hasher;
        _tokens = tokens;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ChildResponseDto> RegisterChildAsync(RegisterChildRequestDto request, CancellationToken ct)
    {
        var userName = NormalizeUserName(request.UserName);
        await EnsureUserNameFreeAsync(userName, ct);

        var child = _mapper.Map<Child>(request);
        child.UserName = userName;
        child.PasswordHash = _hasher.Hash(request.Password);

        await _members.AddAsync(child, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<ChildResponseDto>(child);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct)
    {
        var member = await _members.GetByUserNameAsync(NormalizeUserName(request.UserName), ct)
                     ?? throw new UnauthorizedException(ErrorMessages.LoginFailed);

        return member switch
        {
            Parent parent => await LoginParentAsync(parent, request.Password, ct),
            Child child => LoginChild(child, request.Password),
            _ => throw new UnauthorizedException(ErrorMessages.LoginFailed)
        };
    }

    public async Task<AuthResponseDto> RegisterParentAsync(RegisterParentRequestDto request, CancellationToken ct)
    {
        var access = await GetAccessAsync(ct);
        if (!_hasher.Verify(request.ParentsPassword, access.PasswordHash))
            throw new BadRequestException(ErrorMessages.ParentsPasswordWrong);

        var userName = NormalizeUserName(request.UserName);
        await EnsureUserNameFreeAsync(userName, ct);

        var parent = _mapper.Map<Parent>(request);
        parent.UserName = userName;

        await _members.AddAsync(parent, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return BuildResponse(parent.Id, parent.Name, Roles.Parent, access.MustChangePassword);
    }

    public async Task ChangeParentsPasswordAsync(ChangeParentsPasswordRequestDto request, CancellationToken ct)
    {
        var access = await GetAccessAsync(ct);
        if (!_hasher.Verify(request.CurrentPassword, access.PasswordHash))
            throw new BadRequestException(ErrorMessages.CurrentPasswordWrong);

        access.PasswordHash = _hasher.Hash(request.NewPassword);
        access.MustChangePassword = false;

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<AuthResponseDto> RefreshAsync(CancellationToken ct)
    {
        var member = await _members.GetByIdAsync(_currentUser.UserId, ct)
                     ?? throw new UnauthorizedException(ErrorMessages.UserNotFound);

        if (member is Child { IsApproved: false })
            throw new ForbiddenException(ErrorMessages.ChildPendingApproval);

        var isParent = member is Parent;
        var mustChange = isParent && (await GetAccessAsync(ct)).MustChangePassword;
        var role = isParent ? Roles.Parent : Roles.Child;

        return BuildResponse(member.Id, member.Name, role, mustChange, _currentUser.SessionStart);
    }

    private async Task<AuthResponseDto> LoginParentAsync(Parent parent, string password, CancellationToken ct)
    {
        var access = await GetAccessAsync(ct);
        if (!_hasher.Verify(password, access.PasswordHash))
            throw new UnauthorizedException(ErrorMessages.LoginFailed);

        return BuildResponse(parent.Id, parent.Name, Roles.Parent, access.MustChangePassword);
    }

    private AuthResponseDto LoginChild(Child child, string password)
    {
        if (!_hasher.Verify(password, child.PasswordHash))
            throw new UnauthorizedException(ErrorMessages.LoginFailed);
        if (!child.IsApproved)
            throw new ForbiddenException(ErrorMessages.ChildPendingApproval);

        return BuildResponse(child.Id, child.Name, Roles.Child, mustChangePassword: false);
    }

    private async Task<ParentAccess> GetAccessAsync(CancellationToken ct)
    {
        return await _access.GetSingleAsync(ct) ?? throw new BadRequestException(ErrorMessages.ParentAccessMissing);
    }

    private async Task EnsureUserNameFreeAsync(string userName, CancellationToken ct)
    {
        if (await _members.GetByUserNameAsync(userName, ct) is not null)
            throw new ConflictException(ErrorMessages.UserNameTaken);
    }

    private static string NormalizeUserName(string userName)
    {
        return userName.Trim().ToLowerInvariant();
    }

    private AuthResponseDto BuildResponse(
        int id, string name, string role, bool mustChangePassword, DateTimeOffset? sessionStart = null)
    {
        return new AuthResponseDto
        {
            Token = _tokens.CreateToken(id, name, role, sessionStart),
            Role = role,
            Name = name,
            MustChangePassword = mustChangePassword
        };
    }
}
