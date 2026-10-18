using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Children;

public class ResetChildPasswordRequestDto
{
    [Display(Name = "הסיסמה החדשה")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(ValidationLimits.PasswordMaxLength, MinimumLength = ValidationLimits.PasswordMinLength,
        ErrorMessage = ValidationMessages.LengthBetween)]
    public string NewPassword { get; set; } = string.Empty;
}
