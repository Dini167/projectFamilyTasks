namespace projectFamilyTasks.Application.DTOs.Auth;

public class ChangeParentsPasswordRequestDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
