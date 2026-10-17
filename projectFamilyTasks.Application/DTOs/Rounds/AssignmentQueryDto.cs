using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;

namespace projectFamilyTasks.Application.DTOs.Rounds;

public class AssignmentQueryDto : PagedQueryDto
{
    [Display(Name = "היקף")]
    [EnumDataType(typeof(AssignmentScope), ErrorMessage = ValidationMessages.InvalidChoice)]
    public AssignmentScope Scope { get; set; } = AssignmentScope.Available;
}
