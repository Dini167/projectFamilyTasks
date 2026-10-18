using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs;

public class PagedQueryDto
{
    [Display(Name = "מספר עמוד")]
    [Range(1, ValidationLimits.MaxPageNumber, ErrorMessage = ValidationMessages.RangeBetween)]
    public int Page { get; set; } = 1;

    [Display(Name = "גודל עמוד")]
    [Range(1, 50, ErrorMessage = ValidationMessages.RangeBetween)]
    public int PageSize { get; set; } = 10;

    [Display(Name = "מיון לפי")]
    public string? SortBy { get; set; }
    [Display(Name = "מיון יורד")]
    public bool Descending { get; set; }
}