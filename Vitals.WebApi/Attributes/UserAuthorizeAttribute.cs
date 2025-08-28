namespace Vitals.WebApi.Attributes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

/// <summary>
/// Authorize the resource that can only be accessed by the owner.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class UserAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private const string KeyName = "userId";

    /// <inheritdoc/>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userId = context.HttpContext.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        var routeParameterDictionary = context.HttpContext.GetRouteData();
        var routeUserId = routeParameterDictionary.Values[KeyName]
            ?? throw new InvalidOperationException($"The route has no {KeyName}.");

        if (userId is null)
        {
            goto ending;
        }

        if (routeUserId is string targetUserId && userId.Equals(targetUserId, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

    ending:
        // Using StatusCodeResult instead of ForbidResult to avoid redirect
        // Reference: https://stackoverflow.com/questions/47059015/forbid-returns-404
        context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
    }
}
