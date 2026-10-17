using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Children;

public class UpdateChildRequestDto
{
    [Display(Name = "שם")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [MaxLength(ValidationLimits.NameMaxLength, ErrorMessage = ValidationMessages.MaxLength)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "גיל")]
    [Range(1, ValidationLimits.MaxChildAge, ErrorMessage = ValidationMessages.RangeBetween)]
    public int Age { get; set; }
}