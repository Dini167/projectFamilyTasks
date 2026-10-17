using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Chat;

public class SendMessageRequestDto
{
    [Display(Name = "תוכן ההודעה")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [MaxLength(ValidationLimits.MessageMaxLength, ErrorMessage = ValidationMessages.MaxLength)]
    public string Content { get; set; } = string.Empty;
}