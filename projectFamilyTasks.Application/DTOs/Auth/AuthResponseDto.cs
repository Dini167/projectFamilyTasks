namespace projectFamilyTasks.Application.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool MustChangePassword { get; init; }
}
