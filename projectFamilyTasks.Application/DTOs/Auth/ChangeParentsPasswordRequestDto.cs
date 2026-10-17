using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Auth;

public class ChangeParentsPasswordRequestDto
{
    [Display(Name = "הסיסמה הנוכחית")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Display(Name = "הסיסמה החדשה")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(ValidationLimits.PasswordMaxLength, MinimumLength = ValidationLimits.PasswordMinLength,
        ErrorMessage = ValidationMessages.LengthBetween)]
    public string NewPassword { get; set; } = string.Empty;
}