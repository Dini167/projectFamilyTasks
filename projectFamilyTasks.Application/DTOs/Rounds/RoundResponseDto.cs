using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.DTOs.Rounds;

public class RoundResponseDto
{
    public int Id { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public RoundStatus Status { get; init; }
    public int TasksQuotaPerChild { get; init; }
}
