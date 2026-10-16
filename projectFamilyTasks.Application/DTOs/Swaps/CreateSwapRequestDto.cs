using System.ComponentModel.DataAnnotations;

namespace projectFamilyTasks.Application.DTOs.Swaps;

public class CreateSwapRequestDto
{
    [Range(1, int.MaxValue)]
    public int RequesterAssignmentId { get; set; }

    [Range(1, int.MaxValue)]
    public int TargetAssignmentId { get; set; }
}
