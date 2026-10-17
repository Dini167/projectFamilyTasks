using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.HouseTasks;
using projectFamilyTasks.Application.Interfaces.Services;

namespace projectFamilyTasks.Api.Controllers;


/// <summary>משימות הבית: צפייה לכולם, וניהול להורים.</summary>
public class HouseTasksController : BaseApiController
{
    private readonly IHouseTaskService _tasks;

    public HouseTasksController(IHouseTaskService tasks)
    {
        _tasks = tasks;
    }

    /// <summary>רשימת משימות בעמודים, עם סינון לפי זמן ביום, קטגוריה וגיל, ומיון.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<HouseTaskResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<HouseTaskResponseDto>>> GetAll(
        [FromQuery] HouseTaskQueryDto query, CancellationToken ct)
    {
        return Ok(await _tasks.GetPagedAsync(query, ct));
    }

    /// <summary>משימה אחת לפי מזהה.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(HouseTaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HouseTaskResponseDto>> Get([FromRoute] int id, CancellationToken ct)
    {
        return Ok(await _tasks.GetAsync(id, ct));
    }

    /// <summary>הוספת משימה. להורים בלבד.</summary>
    [Authorize(Roles = Roles.Parent)]
    [HttpPost]
    [ProducesResponseType(typeof(HouseTaskResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<HouseTaskResponseDto>> Create(
        [FromBody] HouseTaskRequestDto request, CancellationToken ct)
    {
        var created = await _tasks.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    /// <summary>עדכון משימה, כולל הקטגוריות שלה. להורים בלבד.</summary>
    [Authorize(Roles = Roles.Parent)]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] int id, [FromBody] HouseTaskRequestDto request, CancellationToken ct)
    {
        await _tasks.UpdateAsync(id, request, ct);
        return NoContent();
    }

    /// <summary>מחיקת משימה. להורים בלבד. משימה שכבר שובצה בסבב לא נמחקת.</summary>
    [Authorize(Roles = Roles.Parent)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        await _tasks.DeleteAsync(id, ct);
        return NoContent();
    }
}
