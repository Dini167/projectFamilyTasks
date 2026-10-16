namespace projectFamilyTasks.Application.DTOs.Children;

public class ChildResponseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public int Age { get; init; }
    public int PointsBalance { get; init; }
    public bool IsApproved { get; init; }
}
