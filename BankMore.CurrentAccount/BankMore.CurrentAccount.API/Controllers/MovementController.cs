using BankMore.Core.Web.Controlles;
using Microsoft.AspNetCore.Mvc;
using BankMore.CurrentAccount.Application.Commands;
using BankMore.CurrentAccount.Domain.Services.Contracts;
using BankMore.CurrentAccount.Domain.Helpers;

namespace BankMore.CurrentAccount.API.Controllers;

[Route("api/movement")]
public sealed class MovementController(IMovementService movementService) : ApplicationController
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateMovementCurrentAccount([FromBody] CurrentAccountMovementCommand command,
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
    {
        var response = await movementService.GetOrSaveMovementAsync(idempotencyKey,
            command.NumberAccount, command.MovementType, command.Value, LoggedNumberAccount);

        if (response.Status == Domain.Enums.MovementOperationEnum.Success)
            return Content(response.PayloadResponse, "application/json");

        if (response.Status == Domain.Enums.MovementOperationEnum.IdempotenceKeyNullOrEmpty)
            return CustomBadRequest(Constants.ApplicationErrors.IdempotenceKeyNullOrEmpty,
                Constants.ApplicationErrors.IdempotenceKeyNullOrEmptyMessage);

        if (response.Status == Domain.Enums.MovementOperationEnum.InvalidAccount)
            return CustomBadRequest(Constants.ApplicationErrors.CurrentAccountInvalid,
                Constants.ApplicationErrors.CurrentAccountInvalidMessage);

        //continuar nas validações de retorno
        //implementar o serviço que vai consumir o tópico (vai salvar o movimento)
        //fazer endpoint de consulta de saldo em IMovementService (usar Dapper)

        return Ok(response);
    }
}