using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Auth;

public class RegisterParentRequestDto
{
    [Display(Name = "שם")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [MaxLength(ValidationLimits.NameMaxLength, ErrorMessage = ValidationMessages.MaxLength)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "שם משתמש")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(ValidationLimits.UserNameMaxLength, MinimumLength = ValidationLimits.UserNameMinLength,
        ErrorMessage = ValidationMessages.LengthBetween)]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "סיסמת ההורים")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    public string ParentsPassword { get; set; } = string.Empty;
}