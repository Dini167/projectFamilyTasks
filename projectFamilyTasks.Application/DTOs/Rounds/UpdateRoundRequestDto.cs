using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.DTOs.Rounds;

public class UpdateRoundRequestDto
{
    public RoundStatus Status { get; set; }
}
