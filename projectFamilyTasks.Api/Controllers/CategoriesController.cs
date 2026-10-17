using Microsoft.AspNetCore.Mvc;
using projectFamilyTasks.Application.DTOs.HouseTasks;
using projectFamilyTasks.Application.Interfaces.Services;

namespace projectFamilyTasks.Api.Controllers;


/// <summary>רשימת הקטגוריות של המשימות.</summary>
public class CategoriesController : BaseApiController
{
    private readonly ICategoryService _categories;

    public CategoriesController(ICategoryService categories)
    {
        _categories = categories;
    }

    /// <summary>כל הקטגוריות לפי שם.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryResponseDto>>> GetAll(CancellationToken ct)
    {
        return Ok(await _categories.GetAllAsync(ct));
    }
}
