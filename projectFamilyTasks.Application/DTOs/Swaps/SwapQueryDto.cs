using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.DTOs.Swaps;

public class SwapQueryDto : PagedQueryDto
{
    [Display(Name = "סטטוס")]
    public SwapStatus? Status { get; set; }
}