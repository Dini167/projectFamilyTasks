using AutoMapper;
using projectFamilyTasks.Application.Common;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Children;
using projectFamilyTasks.Application.Exceptions;
using projectFamilyTasks.Application.Interfaces.Repositories;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Application.Services;

public class ChildService : IChildService
{
    private readonly IFamilyMemberRepository _members;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _hasher;
    private readonly IMapper _mapper;

    public ChildService(
        IFamilyMemberRepository members, IUnitOfWork unitOfWork, IPasswordHasher hasher, IMapper mapper)
    {
        _members = members;
        _unitOfWork = unitOfWork;
        _hasher = hasher;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<ChildResponseDto>> GetPagedAsync(PagedQueryDto query, CancellationToken ct)
    {
        var page = await _members.GetChildrenPagedAsync(query, ct);
        return _mapper.MapPaged<Child, ChildResponseDto>(page);
    }

    public async Task<ChildResponseDto> GetAsync(int id, CancellationToken ct)
    {
        var child = await _members.GetChildAsync(id, ct) ?? throw new NotFoundException(ErrorMessages.ChildNotFound);
        return _mapper.Map<ChildResponseDto>(child);
    }

    public async Task ApproveAsync(int id, CancellationToken ct)
    {
        var child = await GetForUpdateAsync(id, ct);
        if (child.IsApproved)
            return;

        child.IsApproved = true;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(int id, UpdateChildRequestDto request, CancellationToken ct)
    {
        var child = await GetForUpdateAsync(id, ct);
        _mapper.Map(request, child);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var child = await GetForUpdateAsync(id, ct);
        _members.Remove(child);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ResetPasswordAsync(int id, ResetChildPasswordRequestDto request, CancellationToken ct)
    {
        var child = await GetForUpdateAsync(id, ct);
        child.PasswordHash = _hasher.Hash(request.NewPassword);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<Child> GetForUpdateAsync(int id, CancellationToken ct)
    {
        return await _members.GetChildForUpdateAsync(id, ct)
               ?? throw new NotFoundException(ErrorMessages.ChildNotFound);
    }
}
