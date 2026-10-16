using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Auth;

public class RegisterParentRequestDto
{
    [Required, MaxLength(ValidationLimits.NameMaxLength)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(ValidationLimits.UserNameMaxLength, MinimumLength = ValidationLimits.UserNameMinLength)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string ParentsPassword { get; set; } = string.Empty;
}
