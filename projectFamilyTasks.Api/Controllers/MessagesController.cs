using Microsoft.AspNetCore.Mvc;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Chat;
using projectFamilyTasks.Application.Interfaces.Services;

namespace projectFamilyTasks.Api.Controllers;


/// <summary>הצ'אט המשפחתי הפתוח: קריאה ושליחה של הודעות, להורים ולילדים.</summary>
public class MessagesController : BaseApiController
{
    private readonly IMessageService _messages;

    public MessagesController(IMessageService messages)
    {
        _messages = messages;
    }

    /// <summary>ההודעות בצ'אט בעמודים, מהחדשה לישנה.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<MessageResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<MessageResponseDto>>> GetAll(
        [FromQuery] PagedQueryDto query, CancellationToken ct)
    {
        return Ok(await _messages.GetPagedAsync(query, ct));
    }

    /// <summary>שליחת הודעה לצ'אט המשפחתי.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Send([FromBody] SendMessageRequestDto request, CancellationToken ct)
    {
        var message = await _messages.SendAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, message);
    }
}
