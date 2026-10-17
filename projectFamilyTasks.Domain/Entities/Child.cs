using projectFamilyTasks.Domain.Common;

namespace projectFamilyTasks.Domain.Entities;

public class Child : FamilyMember, IHasVersion
{
    public string PasswordHash { get; set; } = string.Empty;
    public int Age { get; set; }
    public int PointsBalance { get; set; }
    public bool IsApproved { get; set; }

    public Guid Version { get; set; }
}
