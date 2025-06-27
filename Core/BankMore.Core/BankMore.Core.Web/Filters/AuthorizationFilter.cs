using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace BankMore.Core.Web.Filters;

internal sealed class AuthorizationFilter : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
    {
        if (ctx.Filters.Any(x => x.GetType() == typeof(AllowAnonymousFilter)))
        {
            await base.OnActionExecutionAsync(ctx, next);
            return;
        }

        var user = ctx.HttpContext.User;
        var isAuthenticated = user?.Identity?.IsAuthenticated ?? false;

        if (!isAuthenticated)
        {
            ctx.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            return;
        }

        var numberAccount = long.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
        var userName = user.FindFirst(ClaimTypes.Name).Value;
        var propertyLoggedNumberAccount = ctx.Controller.GetType().GetProperty("LoggedNumberAccount");
        propertyLoggedNumberAccount.SetValue(ctx.Controller, numberAccount);
        var propertyLoggedPersonName = ctx.Controller.GetType().GetProperty("LoggedPersonName");
        propertyLoggedPersonName.SetValue(ctx.Controller, userName);

        await base.OnActionExecutionAsync(ctx, next);
    }
}