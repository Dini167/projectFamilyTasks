using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Rounds;
using projectFamilyTasks.Application.Interfaces.Services;

namespace projectFamilyTasks.Api.Controllers;


/// <summary>סבבים: פתיחה ושינוי מצב להורים, וצפייה בסבב הנוכחי לכולם.</summary>
public class RoundsController : BaseApiController
{
    private readonly IRoundService _rounds;
    private readonly IAssignmentService _assignments;

    public RoundsController(IRoundService rounds, IAssignmentService assignments)
    {
        _rounds = rounds;
        _assignments = assignments;
    }

    /// <summary>פתיחת סבב חדש עם מכסת משימות לכל ילד. להורים בלבד.</summary>
    [Authorize(Roles = Roles.Parent)]
    [HttpPost]
    [ProducesResponseType(typeof(RoundResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RoundResponseDto>> Open([FromBody] OpenRoundRequestDto request, CancellationToken ct)
    {
        var round = await _rounds.OpenAsync(request, ct);
        return CreatedAtAction(nameof(GetCurrent), null, round);
    }

    /// <summary>שינוי מצב הסבב (למשל סגירת הבחירה). להורים בלבד.</summary>
    [Authorize(Roles = Roles.Parent)]
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] int id, [FromBody] UpdateRoundRequestDto request, CancellationToken ct)
    {
        await _rounds.UpdateAsync(id, request, ct);
        return NoContent();
    }

    /// <summary>הסרת משימה פנויה מהסבב הנוכחי בלבד (המשימה עצמה נשארת). להורים בלבד.</summary>
    [Authorize(Roles = Roles.Parent)]
    [HttpDelete("{roundId:int}/assignments/{assignmentId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveAssignment(
        [FromRoute] int roundId, [FromRoute] int assignmentId, CancellationToken ct)
    {
        await _assignments.RemoveFromRoundAsync(roundId, assignmentId, ct);
        return NoContent();
    }

    /// <summary>רשימת הסבבים בעמודים, מהחדש לישן.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<RoundResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<RoundResponseDto>>> GetAll(
        [FromQuery] PagedQueryDto query, CancellationToken ct)
    {
        return Ok(await _rounds.GetPagedAsync(query, ct));
    }

    /// <summary>הסבב הנוכחי (פתוח לבחירה או פעיל), עם המכסה והמצב.</summary>
    [HttpGet("current")]
    [ProducesResponseType(typeof(RoundResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoundResponseDto>> GetCurrent(CancellationToken ct)
    {
        return Ok(await _rounds.GetCurrentAsync(ct));
    }

    /// <summary>לוח מעקב: לכל ילד משימות, ביצועים ונקודות, ומשימות שנשארו פנויות. להורים בלבד.</summary>
    [Authorize(Roles = Roles.Parent)]
    [HttpGet("{id:int}/summary")]
    [ProducesResponseType(typeof(RoundSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoundSummaryDto>> GetSummary([FromRoute] int id, CancellationToken ct)
    {
        return Ok(await _rounds.GetSummaryAsync(id, ct));
    }
}
