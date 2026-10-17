using System.Security.Claims;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.Interfaces.Services;

namespace projectFamilyTasks.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public int UserId => int.Parse(GetClaim(ClaimTypes.NameIdentifier));

    public string Name => GetClaim(ClaimTypes.Name);

    public string Role => GetClaim(ClaimTypes.Role);

    public DateTimeOffset SessionStart =>
        DateTimeOffset.FromUnixTimeSeconds(long.Parse(GetClaim(AppClaims.SessionStart)));

    private string GetClaim(string type)
    {
        var value = _accessor.HttpContext?.User.FindFirstValue(type);

        return value ?? throw new InvalidOperationException($"No logged-in user claim '{type}' was found.");
    }
}
