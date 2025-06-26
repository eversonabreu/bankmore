using BankMore.Core.Web.Controlles;
using BankMore.CurrentAccount.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.CurrentAccount.API.Controllers;

[Route("api/current-account")]
public class CurrentAccountController(IMediator mediator) : ApplicationController
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateCurrentAccount([FromBody] CreateCurrentAccountCommand command)
    {
        var currentAccount = await mediator.Send(command);
        return Ok(currentAccount);
    }
}