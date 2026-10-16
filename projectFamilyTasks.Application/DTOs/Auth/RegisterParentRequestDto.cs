namespace projectFamilyTasks.Application.DTOs.Auth;

public class RegisterParentRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string ParentsPassword { get; set; } = string.Empty;
}
