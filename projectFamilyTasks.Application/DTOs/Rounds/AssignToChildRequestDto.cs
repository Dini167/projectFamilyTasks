using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Rounds;

public class AssignToChildRequestDto
{
    [Display(Name = "הילד")]
    [Range(1, int.MaxValue, ErrorMessage = ValidationMessages.RangeBetween)]
    public int ChildId { get; set; }
}