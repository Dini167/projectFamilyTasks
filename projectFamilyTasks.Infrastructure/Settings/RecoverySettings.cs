namespace projectFamilyTasks.Infrastructure.Settings;

public class RecoverySettings
{
    public const string SectionName = "Recovery";

    public const int MinKeyLength = 16;

    public string? ParentsPasswordResetKey { get; set; }
}
