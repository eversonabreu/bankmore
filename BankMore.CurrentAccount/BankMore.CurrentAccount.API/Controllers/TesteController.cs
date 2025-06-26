using BankMore.Core.Web.Controlles;
using BankMore.CurrentAccount.Application.Commands;
using BankMore.CurrentAccount.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.CurrentAccount.API.Controllers;

[Route("api/teste")]
public class TesteController(IMediator mediator) : ApplicationController
{
    [HttpGet("get-x"), AllowAnonymous]
    public IActionResult Teste([FromServices] ICurrentAccountRepository act) => Ok(new { act.FirstAsync(x => true).Result });

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateCurrentAccountCommand command)
    {
        var currentAccount = await mediator.Send(command);
        return Ok(currentAccount);
    }
}