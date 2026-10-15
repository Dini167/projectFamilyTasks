using projectFamilyTasks.Domain.Common;

namespace projectFamilyTasks.Domain.Entities;

public abstract class FamilyMember : BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
