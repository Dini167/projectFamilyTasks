using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Auth;

public class RegisterChildRequestDto
{
    [Required, MaxLength(ValidationLimits.NameMaxLength)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(ValidationLimits.UserNameMaxLength, MinimumLength = ValidationLimits.UserNameMinLength)]
    public string UserName { get; set; } = string.Empty;

    [Required, StringLength(ValidationLimits.PasswordMaxLength, MinimumLength = ValidationLimits.PasswordMinLength)]
    public string Password { get; set; } = string.Empty;

    [Range(1, ValidationLimits.MaxChildAge)]
    public int Age { get; set; }
}
