using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Chat;

public class SendMessageRequestDto
{
    [Required, MaxLength(ValidationLimits.MessageMaxLength)]
    public string Content { get; set; } = string.Empty;
}
