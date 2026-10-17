using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.DTOs.HouseTasks;

public class HouseTaskQueryDto : PagedQueryDto
{
    [Display(Name = "זמן ביום")]
    public TimeOfDay? TimeOfDay { get; set; }
    [Display(Name = "קטגוריה")]
    public int? CategoryId { get; set; }
    [Display(Name = "גיל הילד")]
    public int? MaxMinAge { get; set; }
}
