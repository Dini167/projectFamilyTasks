using projectFamilyTasks.Domain.Common;

namespace projectFamilyTasks.Domain.Entities;

public class ParentAccess : BaseEntity
{
    public string PasswordHash { get; set; } = string.Empty;
    public bool MustChangePassword { get; set; }
}
