using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Auth;

public class RegisterChildRequestDto
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

    [Display(Name = "סיסמה")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(ValidationLimits.PasswordMaxLength, MinimumLength = ValidationLimits.PasswordMinLength,
        ErrorMessage = ValidationMessages.LengthBetween)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "גיל")]
    [Range(1, ValidationLimits.MaxChildAge, ErrorMessage = ValidationMessages.RangeBetween)]
    public int Age { get; set; }
}