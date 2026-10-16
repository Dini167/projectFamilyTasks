namespace projectFamilyTasks.Application.DTOs.Auth;

public class RegisterChildRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Age { get; set; }
}
