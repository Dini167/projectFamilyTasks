namespace projectFamilyTasks.Application.DTOs.Rounds;

public class OpenRoundRequestDto
{
    public int TasksQuotaPerChild { get; set; }
    public DateTime? EndDate { get; set; }
}
