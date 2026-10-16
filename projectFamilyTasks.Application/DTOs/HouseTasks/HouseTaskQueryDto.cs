using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.DTOs.HouseTasks;

public class HouseTaskQueryDto : PagedQueryDto
{
    public TimeOfDay? TimeOfDay { get; set; }
    public int? CategoryId { get; set; }
    public int? MaxMinAge { get; set; }
}
