using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Domain.Enums;

namespace projectFamilyTasks.Application.DTOs.HouseTasks;

public class HouseTaskRequestDto
{
    [Display(Name = "תיאור המשימה")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [MaxLength(ValidationLimits.DescriptionMaxLength, ErrorMessage = ValidationMessages.MaxLength)]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "גיל מינימלי")]
    [Range(0, ValidationLimits.MaxChildAge, ErrorMessage = ValidationMessages.RangeBetween)]
    public int MinAge { get; set; }

    [Display(Name = "זמן משוער בדקות")]
    [Range(1, 600, ErrorMessage = ValidationMessages.RangeBetween)]
    public int EstimatedMinutes { get; set; }

    [Display(Name = "זמן ביום")]
    [EnumDataType(typeof(TimeOfDay), ErrorMessage = ValidationMessages.InvalidChoice)]
    public TimeOfDay TimeOfDay { get; set; }

    [Display(Name = "תדירות")]
    [EnumDataType(typeof(Frequency), ErrorMessage = ValidationMessages.InvalidChoice)]
    public Frequency Frequency { get; set; }

    [Display(Name = "ערך הנקודות")]
    [Range(1, 1000, ErrorMessage = ValidationMessages.RangeBetween)]
    public int PointsValue { get; set; }

    [Display(Name = "קטגוריות")]
    public List<int> CategoryIds { get; set; } = new();
}