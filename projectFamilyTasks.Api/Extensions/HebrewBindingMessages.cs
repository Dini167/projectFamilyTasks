using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace projectFamilyTasks.Api.Extensions;

public static class HebrewBindingMessages
{
    public static void Apply(DefaultModelBindingMessageProvider provider)
    {
        provider.SetValueMustNotBeNullAccessor(name => $"יש למלא את השדה {name}.");
        provider.SetMissingBindRequiredValueAccessor(name => $"יש לשלוח את השדה {name}.");
        provider.SetMissingKeyOrValueAccessor(() => "חסר מפתח או ערך.");
        provider.SetMissingRequestBodyRequiredValueAccessor(() => "יש לשלוח נתונים בגוף הבקשה.");
        provider.SetValueIsInvalidAccessor(value => "הערך שהוזן אינו תקין.");
        provider.SetAttemptedValueIsInvalidAccessor((value, name) => $"הערך שהוזן בשדה {name} אינו תקין.");
        provider.SetUnknownValueIsInvalidAccessor(name => $"הערך שהוזן בשדה {name} אינו תקין.");
        provider.SetValueMustBeANumberAccessor(name => $"בשדה {name} יש להזין מספר.");
        provider.SetNonPropertyAttemptedValueIsInvalidAccessor(value => "הערך שהוזן אינו תקין.");
        provider.SetNonPropertyUnknownValueIsInvalidAccessor(() => "הערך שהוזן אינו תקין.");
        provider.SetNonPropertyValueMustBeANumberAccessor(() => "יש להזין מספר.");
    }
}