using BankMore.Core.Web.Controlles;
using Microsoft.AspNetCore.Mvc;
using BankMore.CurrentAccount.Domain.Services.Contracts;
using BankMore.CurrentAccount.Domain.Helpers;
using BankMore.CurrentAccount.Application.Requests;

namespace BankMore.CurrentAccount.API.Controllers;

[Route("api/movement")]
public sealed class MovementController(IMovementService movementService) : ApplicationController
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateMovementCurrentAccount([FromBody] MovementRequest command,
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
    {
        var (Status, PayloadResponse) = await movementService.GetOrSaveMovementAsync(idempotencyKey,
            command.NumberAccount, command.MovementType, command.Value, LoggedNumberAccount);

        return Status switch
        {
            Domain.Enums.MovementOperationEnum.Success => new ContentResult
            {
                Content = PayloadResponse,
                StatusCode = 200,
                ContentType = "application/json"
            },

            Domain.Enums.MovementOperationEnum.IdempotenceKeyNullOrEmpty =>
                CustomBadRequest(Constants.ApplicationMessages.IdempotenceKeyNullOrEmptyError,
                    Constants.ApplicationMessages.IdempotenceKeyNullOrEmptyErrorMessage),

            Domain.Enums.MovementOperationEnum.InvalidAccount =>
                CustomBadRequest(Constants.ApplicationMessages.CurrentAccountInvalid,
                    Constants.ApplicationMessages.CurrentAccountInvalidMessage),

            Domain.Enums.MovementOperationEnum.InactiveAccount =>
                CustomBadRequest(Constants.ApplicationMessages.CurrentAccountInactive,
                    Constants.ApplicationMessages.CurrentAccountInactiveMessage),

            Domain.Enums.MovementOperationEnum.IdempotenceRequestBodyMismatch =>
                CustomBadRequest(Constants.ApplicationMessages.IdempotenceRequestBodyMismatchError,
                    Constants.ApplicationMessages.CurrentAccountInvalidMessage),

            Domain.Enums.MovementOperationEnum.WaitingFinishProccess =>
                Accepted(new { Message = Constants.ApplicationMessages.MovementWaitingFinishProccess }),

            _ => CustomBadRequest(Constants.ApplicationMessages.FatalErrorMovementCurrentAccountError,
                Constants.ApplicationMessages.FatalErrorMovementCurrentAccountErrorMessage)
        };
    }

    //implementar o serviço que vai consumir o tópico (vai salvar o movimento)
    //fazer endpoint de consulta de saldo em IMovementService (usar Dapper)
}