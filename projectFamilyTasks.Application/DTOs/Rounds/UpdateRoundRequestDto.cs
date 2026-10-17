using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.DTOs.Rounds;

public class UpdateRoundRequestDto
{
    [Display(Name = "סטטוס הסבב")]
    [EnumDataType(typeof(RoundStatus), ErrorMessage = ValidationMessages.InvalidChoice)]
    public RoundStatus Status { get; set; }
}