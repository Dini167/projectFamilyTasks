namespace projectFamilyTasks.Application.Interfaces.Services;

public interface IRecoveryKeyValidator
{
    bool IsValid(string? key);
}
