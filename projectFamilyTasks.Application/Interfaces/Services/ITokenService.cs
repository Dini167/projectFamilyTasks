namespace projectFamilyTasks.Application.Interfaces.Services;

public interface ITokenService
{
    string CreateToken(int userId, string name, string role, DateTimeOffset? sessionStart = null);
}
