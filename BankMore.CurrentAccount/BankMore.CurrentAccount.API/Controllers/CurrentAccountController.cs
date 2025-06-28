using BankMore.Core.Web.Controlles;
using BankMore.CurrentAccount.Application.Commands;
using BankMore.CurrentAccount.Domain.Helpers;
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

    [HttpPut("deactivate")]
    public async Task<IActionResult> DeactivateCurrentAccount([FromBody] DeactivateCurrentAccountCommand command)
    {
        var result = await mediator.Send(command);
        
        if (result)
        {
            return NoContent();
        }

        return CustomBadRequest("A conta-corrente não foi encontrada ou já estava inativa.",
            Constants.ApplicationMessages.FailDeactivateCurrentAccount);
    }

    // como não foi solicitado na especificação, vou suprimir os demais endpoints
    // que fariam a atualização dos dados cadastrais da conta corrente (alteração de nome, reativação de conta, senha...)
}