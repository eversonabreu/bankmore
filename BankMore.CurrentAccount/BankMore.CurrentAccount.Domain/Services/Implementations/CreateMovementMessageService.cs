using BankMore.Core.Infrastructure.Messaging;
using BankMore.CurrentAccount.Domain.Dtos;
using BankMore.CurrentAccount.Domain.Helpers;
using BankMore.CurrentAccount.Domain.Repositories;
using BankMore.CurrentAccount.Domain.Services.Contracts;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BankMore.CurrentAccount.Domain.Services.Implementations;

public sealed class CreateMovementMessageService(IMovementRepository movementRepository,
    IIdempotenceService idempotenceService, ILogger<CreateMovementMessageService> logger) : IMessageTopicHandler
{
    public async Task HandleMessageAsync(string payload)
        => await HandleAsync(JsonSerializer.Deserialize<IdempotenceRequestDto<MovementDto>>(payload));

    public async Task<string> HandleAsync(object body)
    {
        var payload = (IdempotenceRequestDto<MovementDto>)body;

        try
        {
            var movementOperation = await movementRepository.CreateMovementAsync(payload.PayloadRequisition.NumberAccount,
                payload.PayloadRequisition.MovementType, payload.PayloadRequisition.Value);

            if (movementOperation == Enums.MovementOperationEnum.Success)
            {
                return await idempotenceService.SavePayloadResponseAsync(payload.IdempotenceKey, new
                {
                    Status = $"Movimentação na conta-correnta realizada com sucesso em '{DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}'.",
                    MovementValue = payload.PayloadRequisition.Value,
                    payload.PayloadRequisition.NumberAccount,
                    MovementOperation = payload.PayloadRequisition.MovementType == Enums.MovementTypeEnum.Credit ? "Crédito" : "Débito"
                });
            }

            if (movementOperation == Enums.MovementOperationEnum.InvalidValue)
                throw new InvalidOperationException(Constants.ApplicationMessages.FailMovementValue);

            if (movementOperation == Enums.MovementOperationEnum.InvalidAccount)
                throw new InvalidOperationException(Constants.ApplicationMessages.CurrentAccountInvalid);

            if (movementOperation == Enums.MovementOperationEnum.InactiveAccount)
                throw new InvalidOperationException(Constants.ApplicationMessages.CurrentAccountInactive);

            // default message error
            throw new Exception("Fatal error processing. Verify logs for determines of the cause problem.");
        }
        catch (Exception exc)
        {
            string message = $"{Constants.ApplicationMessages.FatalErrorMovementCurrentAccountError}|{exc.Message}";

            logger.LogError(exc, "Fatal error processing. {Message}", exc.Message);

            return await idempotenceService.SavePayloadResponseAsync(payload.IdempotenceKey, new
            {
                Status = message
            });
        }
    }
}