using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Auth;

public class LoginRequestDto
{
    [Display(Name = "שם משתמש")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "סיסמה")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    public string Password { get; set; } = string.Empty;
}