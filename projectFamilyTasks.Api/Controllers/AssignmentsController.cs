using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Rounds;
using projectFamilyTasks.Application.Interfaces.Services;

namespace projectFamilyTasks.Api.Controllers;


/// <summary>שיבוצים: רשימות, תפיסת משימה וסימון ביצוע, לילדים בלבד.</summary>
[Authorize(Roles = Roles.Child)]
public class AssignmentsController : BaseApiController
{
    private readonly IAssignmentService _assignments;
    private readonly ICompletionService _completions;

    public AssignmentsController(IAssignmentService assignments, ICompletionService completions)
    {
        _assignments = assignments;
        _completions = completions;
    }

    /// <summary>רשימת שיבוצים: scope=available פנויות שמתאימות לגיל, scope=mine שלי, או scope=siblings משימות שנבחרו על ידי אחים (לבקשת החלפה).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<AssignmentResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<AssignmentResponseDto>>> GetAll(
        [FromQuery] AssignmentQueryDto query, CancellationToken ct)
    {
        var page = query.Scope switch
        {
            AssignmentScope.Mine => await _assignments.GetMineAsync(query, ct),
            AssignmentScope.Siblings => await _assignments.GetSiblingsAsync(query, ct),
            _ => await _assignments.GetAvailableAsync(query, ct)
        };

        return Ok(page);
    }

    /// <summary>תפיסת משימה פנויה (ראשון זוכה). אם ילד אחר הקדים: 409.</summary>
    [HttpPut("{id:int}/assignee")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Take([FromRoute] int id, CancellationToken ct)
    {
        await _assignments.TakeAsync(id, ct);
        return NoContent();
    }

    /// <summary>סימון "ביצעתי!" למשימה ששובצה לי. הנקודות נוספות למאזן.</summary>
    [HttpPost("{id:int}/completions")]
    [ProducesResponseType(typeof(CompletionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Complete([FromRoute] int id, CancellationToken ct)
    {
        var completion = await _completions.CompleteAsync(id, ct);
        return StatusCode(StatusCodes.Status201Created, completion);
    }
}
