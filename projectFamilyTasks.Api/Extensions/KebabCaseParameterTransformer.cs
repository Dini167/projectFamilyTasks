using System.Text.RegularExpressions;

namespace projectFamilyTasks.Api.Extensions;

public class KebabCaseParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        return value is null
            ? null
            : Regex.Replace(value.ToString()!, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
    }
}
