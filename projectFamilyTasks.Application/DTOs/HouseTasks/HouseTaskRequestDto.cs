using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.DTOs.HouseTasks;

public class HouseTaskRequestDto
{
    [Required, MaxLength(ValidationLimits.DescriptionMaxLength)]
    public string Description { get; set; } = string.Empty;

    [Range(0, ValidationLimits.MaxChildAge)]
    public int MinAge { get; set; }

    [Range(1, 600)]
    public int EstimatedMinutes { get; set; }

    [EnumDataType(typeof(TimeOfDay))]
    public TimeOfDay TimeOfDay { get; set; }

    [EnumDataType(typeof(Frequency))]
    public Frequency Frequency { get; set; }

    [Range(1, 1000)]
    public int PointsValue { get; set; }

    public List<int> CategoryIds { get; set; } = new();
}
