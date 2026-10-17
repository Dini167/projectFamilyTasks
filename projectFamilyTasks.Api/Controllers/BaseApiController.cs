using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectFamilyTasks.Application.DTOs;

namespace projectFamilyTasks.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
public abstract class BaseApiController : ControllerBase
{
}
