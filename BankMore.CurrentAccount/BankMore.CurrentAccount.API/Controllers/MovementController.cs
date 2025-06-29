using BankMore.Core.Web.Controlles;
using Microsoft.AspNetCore.Mvc;
using BankMore.CurrentAccount.Domain.Services.Contracts;
using BankMore.CurrentAccount.Domain.Helpers;
using BankMore.CurrentAccount.Application.Requests;
using BankMore.CurrentAccount.Domain.Enums;

namespace BankMore.CurrentAccount.API.Controllers;

[Route("api/movement")]
public sealed class MovementController(IMovementService movementService) : ApplicationController
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateMovementCurrentAccount([FromBody] MovementRequest command,
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
    {
        var movementType = command.MovementType.Equals("C", StringComparison.InvariantCultureIgnoreCase) 
            ? MovementTypeEnum.Credit : MovementTypeEnum.Debit;

        var (Status, PayloadResponse) = await movementService.GetOrSaveMovementAsync(idempotencyKey,
            command.NumberAccount, movementType, command.Value, LoggedNumberAccount);

        return Status switch
        {
            MovementOperationEnum.Success => new ContentResult
            {
                Content = PayloadResponse,
                StatusCode = 200,
                ContentType = "application/json"
            },

            MovementOperationEnum.IdempotenceKeyNullOrEmpty =>
                CustomBadRequest(Constants.ApplicationMessages.IdempotenceKeyNullOrEmptyError,
                    Constants.ApplicationMessages.IdempotenceKeyNullOrEmptyErrorMessage),

            MovementOperationEnum.InvalidType =>
                CustomBadRequest(Constants.ApplicationMessages.FailMovementType,
                    Constants.ApplicationMessages.FailMovementTypeMessage),

            MovementOperationEnum.InvalidAccount =>
                CustomBadRequest(Constants.ApplicationMessages.CurrentAccountInvalid,
                    Constants.ApplicationMessages.CurrentAccountInvalidMessage),

            MovementOperationEnum.InactiveAccount =>
                CustomBadRequest(Constants.ApplicationMessages.CurrentAccountInactive,
                    Constants.ApplicationMessages.CurrentAccountInactiveMessage),

            MovementOperationEnum.IdempotenceRequestBodyMismatch =>
                CustomBadRequest(Constants.ApplicationMessages.IdempotenceRequestBodyMismatchError,
                    Constants.ApplicationMessages.IdempotenceRequestBodyMismatchMessage),

            MovementOperationEnum.WaitingFinishProccess =>
                Accepted(new { Message = Constants.ApplicationMessages.MovementWaitingFinishProccess }),

            _ => CustomBadRequest(Constants.ApplicationMessages.FatalErrorMovementCurrentAccountError,
                Constants.ApplicationMessages.FatalErrorMovementCurrentAccountErrorMessage)
        };
    }

    [HttpGet("balance/{numberAccount:long}")]
    public async Task<IActionResult> GetBalanceAsync([FromRoute] long numberAccount)
    {
        var (Balance, Status) = await movementService.GetBalanceAsync(numberAccount);

        return Status switch
        {
            MovementOperationEnum.InvalidAccount =>
                CustomBadRequest(Constants.ApplicationMessages.CurrentAccountInvalid,
                    Constants.ApplicationMessages.CurrentAccountInvalidMessage),

            MovementOperationEnum.InactiveAccount =>
                CustomBadRequest(Constants.ApplicationMessages.CurrentAccountInactive,
                    Constants.ApplicationMessages.CurrentAccountInactiveMessage),

            _ => Ok(Balance)
        };
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferAsync([FromBody] TransferDto transferDto,
        [FromHeader(Name = "Authorization")] string authorizationHeader)
    {
        var (IsSuccess, MessageError) = await movementService.TransferAsync(transferDto.NumberAccountOrigin, 
            transferDto.NumberAccountDestination, transferDto.Value, authorizationHeader);

        if (IsSuccess)
            return Ok();

        return BadRequest(MessageError);
    }
}