using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.DTOs;
using projectFamilyTasks.Application.DTOs.Children;
using projectFamilyTasks.Application.Interfaces.Services;

namespace projectFamilyTasks.Api.Controllers;

/// <summary>הפרטים של הילד המחובר, כולל מאזן הנקודות.</summary>
[Authorize(Roles = Roles.Child)]
[Route("api/children/me")]
public class MyProfileController : BaseApiController
{
    private readonly IChildService _children;
    private readonly ICurrentUserService _currentUser;

    public MyProfileController(IChildService children, ICurrentUserService currentUser)
    {
        _children = children;
        _currentUser = currentUser;
    }

    /// <summary>הפרטים של הילד המחובר.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ChildResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChildResponseDto>> Get(CancellationToken ct)
    {
        return Ok(await _children.GetAsync(_currentUser.UserId, ct));
    }
}
