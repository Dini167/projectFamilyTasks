using System.ComponentModel.DataAnnotations;

namespace projectFamilyTasks.Application.DTOs.Rounds;

public class OpenRoundRequestDto
{
    [Range(1, 20)]
    public int TasksQuotaPerChild { get; set; }

    public DateTime? EndDate { get; set; }
}
