using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace BankMore.Core.Web.Filters;

internal sealed class AuthorizationFilter : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
    {
        var user = ctx.HttpContext.User;
        var isAuthenticated = user?.Identity?.IsAuthenticated ?? false;

        if (!isAuthenticated)
        {
            ctx.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return;
        }

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = user.FindFirst(ClaimTypes.Name)?.Value;

        ctx.HttpContext.Items["UserId"] = userId;
        ctx.HttpContext.Items["UserName"] = userName;

        await base.OnActionExecutionAsync(ctx, next);
    }
}