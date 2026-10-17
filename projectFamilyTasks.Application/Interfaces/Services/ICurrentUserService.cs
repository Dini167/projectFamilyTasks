namespace projectFamilyTasks.Application.Interfaces.Services;

public interface ICurrentUserService
{
    int UserId { get; }

    string Name { get; }

    string Role { get; }

    DateTimeOffset SessionStart { get; }
}
