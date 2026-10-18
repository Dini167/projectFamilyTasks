using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Children;
using projectFamilyTasks.Application.Interfaces.Services;

namespace projectFamilyTasks.Api.Controllers;


/// <summary>ניהול הילדים על ידי ההורים.</summary>
[Authorize(Roles = Roles.Parent)]
public class ChildrenController : BaseApiController
{
    private readonly IChildService _children;
    private readonly IAssignmentService _assignments;

    public ChildrenController(IChildService children, IAssignmentService assignments)
    {
        _children = children;
        _assignments = assignments;
    }

    /// <summary>רשימת הילדים בעמודים, כולל אלה שממתינים לאישור.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ChildResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<ChildResponseDto>>> GetAll(
        [FromQuery] PagedQueryDto query, CancellationToken ct)
    {
        return Ok(await _children.GetPagedAsync(query, ct));
    }

    /// <summary>ילד אחד לפי מזהה.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ChildResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChildResponseDto>> Get([FromRoute] int id, CancellationToken ct)
    {
        return Ok(await _children.GetAsync(id, ct));
    }

    /// <summary>עדכון שם וגיל של ילד.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] int id, [FromBody] UpdateChildRequestDto request, CancellationToken ct)
    {
        await _children.UpdateAsync(id, request, ct);
        return NoContent();
    }

    /// <summary>הורה קובע סיסמה חדשה לילד ששכח את הסיסמה שלו.</summary>
    [HttpPut("{id:int}/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword(
        [FromRoute] int id, [FromBody] ResetChildPasswordRequestDto request, CancellationToken ct)
    {
        await _children.ResetPasswordAsync(id, request, ct);
        return NoContent();
    }

    /// <summary>הסרת ילד, לשימוש בהרשמה כפולה בטעות.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        await _children.DeleteAsync(id, ct);
        return NoContent();
    }

    /// <summary>אישור הרשמת ילד. אין דחייה: ילד שממתין פשוט מחכה לאישור.</summary>
    [HttpPut("{id:int}/approval")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve([FromRoute] int id, CancellationToken ct)
    {
        await _children.ApproveAsync(id, ct);
        return NoContent();
    }

    /// <summary>שיבוץ משימה לילד שלא בחר. חלים אותם כללי גיל ומכסה.</summary>
    [HttpPut("{childId:int}/assignments/{assignmentId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignTask(
        [FromRoute] int childId, [FromRoute] int assignmentId, CancellationToken ct)
    {
        await _assignments.AssignToChildAsync(assignmentId, childId, ct);
        return NoContent();
    }
}
