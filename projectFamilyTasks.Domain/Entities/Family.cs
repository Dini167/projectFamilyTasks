using projectFamilyTasks.Domain.Common;

namespace projectFamilyTasks.Domain.Entities;

public class Family : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}
