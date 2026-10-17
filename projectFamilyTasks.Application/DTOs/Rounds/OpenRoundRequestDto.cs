using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Rounds;

public class OpenRoundRequestDto
{
    [Display(Name = "מכסת משימות לילד")]
    [Range(1, 20, ErrorMessage = ValidationMessages.RangeBetween)]
    public int TasksQuotaPerChild { get; set; }

    [Display(Name = "תאריך סיום")]
    public DateTime? EndDate { get; set; }
}