using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using projectFamilyTasks.Application.Interfaces.Services;
using projectFamilyTasks.Infrastructure.Settings;

namespace projectFamilyTasks.Infrastructure.Security;

public class RecoveryKeyValidator : IRecoveryKeyValidator
{
    private readonly RecoverySettings _settings;

    public RecoveryKeyValidator(IOptions<RecoverySettings> settings)
    {
        _settings = settings.Value;
    }

    public bool IsValid(string? key)
    {
        var configured = _settings.ParentsPasswordResetKey;
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(configured))
            return false;
        if (configured.Length < RecoverySettings.MinKeyLength)
            return false;

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(configured));
    }
}
