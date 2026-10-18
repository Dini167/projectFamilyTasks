namespace projectFamilyTasks.Api.Middleware;

// מסמן נקודת קצה שמותרת להורה גם כשעדיין חייבים להחליף את סיסמת ההורים ההתחלתית.
[AttributeUsage(AttributeTargets.Method)]
public class AllowWhenPasswordChangeRequiredAttribute : Attribute
{
}
