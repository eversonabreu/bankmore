using BankMore.Core.Web.Controlles;
using BankMore.CurrentAccount.Application.Commands;
using BankMore.CurrentAccount.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.CurrentAccount.API.Controllers;

[Route("api/auth")]
public sealed class LoginController : ApplicationController
{
    [HttpPost("login"), AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromServices] IMediator mediator, [FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);

        if (result.LoginStatus == Domain.Enums.LoginStatusEnum.Success)
        {
            return Ok(new
            {
                Token = result.JwtToken,
                result.NumberAccount,
                result.PersonName
            });
        }

        return CustomUnauthorized(
            result.LoginStatus == Domain.Enums.LoginStatusEnum.FailWhenManyPersonDocument 
                ? Constants.ApplicationMessages.FailLoginUserWhenManyPersonDocument
                : (result.LoginStatus == Domain.Enums.LoginStatusEnum.FailWhenUserInactive
                    ? Constants.ApplicationMessages.FailLoginUserWhenUserInactive
                    : Constants.ApplicationMessages.FailLoginUserWhenUserNotFound),
            Constants.ApplicationMessages.FailLoginUser);
    }
}