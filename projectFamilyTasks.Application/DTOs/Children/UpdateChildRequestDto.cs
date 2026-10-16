using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Children;

public class UpdateChildRequestDto
{
    [Required, MaxLength(ValidationLimits.NameMaxLength)]
    public string Name { get; set; } = string.Empty;

    [Range(1, ValidationLimits.MaxChildAge)]
    public int Age { get; set; }
}
