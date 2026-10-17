using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Swaps;

public class CreateSwapRequestDto
{
    [Display(Name = "המשימה להחלפה")]
    [Range(1, int.MaxValue, ErrorMessage = ValidationMessages.RangeBetween)]
    public int RequesterAssignmentId { get; set; }

    [Display(Name = "המשימה של האח")]
    [Range(1, int.MaxValue, ErrorMessage = ValidationMessages.RangeBetween)]
    public int TargetAssignmentId { get; set; }
}