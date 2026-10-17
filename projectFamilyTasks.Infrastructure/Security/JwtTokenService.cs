using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Infrastructure.Settings;

namespace projectFamilyTasks.Infrastructure.Security;

public class JwtTokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public string CreateToken(int userId, string name, string role, DateTimeOffset? sessionStart = null)
    {
        var now = DateTimeOffset.UtcNow;
        var start = sessionStart ?? now;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Role, role),
            new Claim(AppClaims.SessionStart, start.ToUnixTimeSeconds().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var idleExpiry = now.AddMinutes(_settings.ExpiryMinutes);
        var sessionEnd = start.AddHours(_settings.MaxSessionHours);
        var expires = idleExpiry < sessionEnd ? idleExpiry : sessionEnd;

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expires.UtcDateTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
