namespace projectFamilyTasks.Infrastructure.Settings;

public class SeedSettings
{
    public const string SectionName = "Seed";

    public string? InitialParentsPassword { get; set; }

    public string? DemoPassword { get; set; }
}
