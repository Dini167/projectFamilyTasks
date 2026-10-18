using projectFamilyTasks.Application.DTOs.Auth;
using projectFamilyTasks.Application.DTOs.Children;

namespace projectFamilyTasks.Application.Interfaces.Services;

public interface IAuthService
{
    Task<ChildResponseDto> RegisterChildAsync(RegisterChildRequestDto request, CancellationToken ct);

    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct);

    Task<AuthResponseDto> RegisterParentAsync(RegisterParentRequestDto request, CancellationToken ct);

    Task ChangeParentsPasswordAsync(ChangeParentsPasswordRequestDto request, CancellationToken ct);
    Task ResetParentsPasswordAsync(ResetParentsPasswordRequestDto request, CancellationToken ct);

    Task<AuthResponseDto> RefreshAsync(CancellationToken ct);
}
