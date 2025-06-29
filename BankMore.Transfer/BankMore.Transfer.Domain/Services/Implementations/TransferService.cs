﻿using BankMore.Core.Infrastructure.Messaging;
using BankMore.Transfer.Domain.Dtos;
using BankMore.Transfer.Domain.Enums;
using BankMore.Transfer.Domain.Repositories;
using BankMore.Transfer.Domain.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BankMore.Transfer.Domain.Services.Implementations;

public sealed class TransferService(ITransferRepository transferRepository,
    IServiceProvider serviceProvider, ILogger<TransferService> logger) : ITransferService
{
    public async Task<MovementOperationEnum> CreateTransferAsync(TransferDto transferDto)
    {
        if (transferDto.CurrentAccountOriginId == transferDto.CurrentAccountDestinationId)
            return MovementOperationEnum.InvalidAccountDestination;

        if (transferDto.TransferValue <= 0m)
            return MovementOperationEnum.InvalidValue;

        var transfer = await transferRepository.CreateAsync(new Entities.Transfer
        {
            TransferValue = transferDto.TransferValue,
            CurrentAccountDestinationId = transferDto.CurrentAccountDestinationId,
            CurrentAccountOriginId = transferDto.CurrentAccountOriginId,
            TransferDate = DateTime.UtcNow
        });

        await transferRepository.SaveChangesAsync();
        _ = CreateMessageToRateRegistrationAsync(transfer.Id, transferDto.CurrentAccountOriginId);

        return MovementOperationEnum.Success;
    }

    public async Task<IReadOnlyCollection<Entities.Transfer>> GetTransfersByCurrentAccount(Guid currentAccountId)
        => await transferRepository.GetAsync(x => x.CurrentAccountOriginId ==  currentAccountId);

    private async Task CreateMessageToRateRegistrationAsync(Guid transferId, Guid currentAccountOriginId)
    {
        try
        {
            logger.LogInformation("Publish message for save rate in transfer. TransferId: {TransferId}", transferId);
            using var scope = serviceProvider.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IMessageService>();
            await publisher.PublishAsync(Topics.CurrentAccountTransferTopicName, 
                new { TransferId = transferId, OriginId = currentAccountOriginId });
        }
        catch (Exception exc) 
        {
            logger.LogError(exc, "Error when publish message for save rate in transfer. {Message}", exc.Message);
            throw;  // it's necessary because musen't be mark the message as consumed
        }
    }
}