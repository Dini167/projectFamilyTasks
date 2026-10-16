using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.DTOs.Rounds;

public class UpdateRoundRequestDto
{
    [EnumDataType(typeof(RoundStatus))]
    public RoundStatus Status { get; set; }
}
