using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Auth;

public class ResetParentsPasswordRequestDto
{
    [Display(Name = "מפתח השחזור")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    public string RecoveryKey { get; set; } = string.Empty;

    [Display(Name = "הסיסמה החדשה")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(ValidationLimits.PasswordMaxLength, MinimumLength = ValidationLimits.PasswordMinLength,
        ErrorMessage = ValidationMessages.LengthBetween)]
    public string NewPassword { get; set; } = string.Empty;
}
