using projectFamilyTasks.Application.Common.Constants;
using projectFamilyTasks.Application.Interfaces.Repositories;

namespace projectFamilyTasks.Api.Middleware;

public class PasswordChangeRequiredMiddleware
{
    private readonly RequestDelegate _next;

    public PasswordChangeRequiredMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IParentAccessRepository access)
    {
        if (IsParentOnRestrictedEndpoint(context) && await MustChangePasswordAsync(access, context))
        {
            await ErrorResponseWriter.WriteAsync(
                context, StatusCodes.Status403Forbidden, ErrorMessages.PasswordChangeRequired);
            return;
        }

        await _next(context);
    }

    private static bool IsParentOnRestrictedEndpoint(HttpContext context)
    {
        var isParent = context.User.Identity?.IsAuthenticated == true && context.User.IsInRole(Roles.Parent);
        var metadata = context.GetEndpoint()?.Metadata;
        var allowed = metadata?.GetMetadata<AllowWhenPasswordChangeRequiredAttribute>() is not null;

        return isParent && !allowed;
    }

    private static async Task<bool> MustChangePasswordAsync(IParentAccessRepository access, HttpContext context)
    {
        var parentAccess = await access.GetSingleAsync(context.RequestAborted);
        return parentAccess?.MustChangePassword == true;
    }
}
