using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Swaps;
using projectFamilyTasks.Application.Interfaces.Services;

namespace projectFamilyTasks.Api.Controllers;


/// <summary>בקשות החלפה בין אחים, עם אישור של האח ואחריו של הורה.</summary>
public class SwapRequestsController : BaseApiController
{
    private readonly ISwapService _swaps;

    public SwapRequestsController(ISwapService swaps)
    {
        _swaps = swaps;
    }

    /// <summary>בקשת החלפה בין המשימה שלי למשימה של אח. לילדים בלבד.</summary>
    [Authorize(Roles = Roles.Child)]
    [HttpPost]
    [ProducesResponseType(typeof(SwapRequestResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSwapRequestDto request, CancellationToken ct)
    {
        var swap = await _swaps.CreateAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, swap);
    }

    /// <summary>רשימת בקשות החלפה, עם סינון לפי סטטוס. הורה רואה הכול, וילד רק בקשות שהוא מעורב בהן.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<SwapRequestResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<SwapRequestResponseDto>>> GetAll(
        [FromQuery] SwapQueryDto query, CancellationToken ct)
    {
        return Ok(await _swaps.GetPagedAsync(query, ct));
    }

    /// <summary>החלטה על בקשה: האח בשלב הראשון, הורה בשלב השני. אישור או דחייה.</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Decide(
        [FromRoute] int id, [FromBody] SwapDecisionRequestDto request, CancellationToken ct)
    {
        await _swaps.DecideAsync(id, request, ct);
        return NoContent();
    }
}
