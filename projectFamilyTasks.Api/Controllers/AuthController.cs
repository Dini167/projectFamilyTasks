using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using projectFamilyTasks.Api.Extensions;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Auth;
using projectFamilyTasks.Application.DTOs.Children;
using projectFamilyTasks.Application.Interfaces.Services;

namespace projectFamilyTasks.Api.Controllers;


/// <summary>הרשמה, התחברות, החלפת סיסמת הורים וחידוש טוקן.</summary>
public class AuthController : BaseApiController
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    /// <summary>הרשמת ילד. החשבון ממתין לאישור של אחד ההורים.</summary>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(ChildResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterChildRequestDto request, CancellationToken ct)
    {
        var child = await _auth.RegisterChildAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, child);
    }

    /// <summary>התחברות הורה (עם סיסמת ההורים) או ילד (עם סיסמה אישית). מחזירה טוקן.</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken ct)
    {
        return Ok(await _auth.LoginAsync(request, ct));
    }

    /// <summary>הרשמת הורה נוסף. נדרשת סיסמת ההורים המשותפת.</summary>
    [AllowAnonymous]
    [HttpPost("register-parent")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterParent([FromBody] RegisterParentRequestDto request, CancellationToken ct)
    {
        var response = await _auth.RegisterParentAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>שחזור סיסמת ההורים עם מפתח חירום שהוגדר בשרת. מוגבל בקצב ניסיונות.</summary>
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.RecoveryPolicy)]
    [HttpPost("reset-parents-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ResetParentsPassword(
        [FromBody] ResetParentsPasswordRequestDto request, CancellationToken ct)
    {
        await _auth.ResetParentsPasswordAsync(request, ct);
        return NoContent();
    }

    /// <summary>החלפת סיסמת ההורים המשותפת. להורים בלבד.</summary>
    [Authorize(Roles = Roles.Parent)]
    [HttpPut("parents-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangeParentsPassword(
        [FromBody] ChangeParentsPasswordRequestDto request, CancellationToken ct)
    {
        await _auth.ChangeParentsPasswordAsync(request, ct);
        return NoContent();
    }

    /// <summary>חידוש הטוקן, בלי להאריך מעבר ל-5 שעות מההתחברות המקורית.</summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponseDto>> Refresh(CancellationToken ct)
    {
        return Ok(await _auth.RefreshAsync(ct));
    }
}
