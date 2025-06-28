using BankMore.Core.Infrastructure.Messaging;
using BankMore.CurrentAccount.Domain.Enums;

namespace BankMore.CurrentAccount.Domain.Services.Contracts;

public interface IIdempotenceService
{
    public Task<(MovementOperationEnum Status, string PayloadResponse)> ProcessAsync<TValue>(
        string idempotenceKey,
        TValue payloadRequest,
        string messageTopicName,
        IMessageTopicHandler messageTopicHandler);
}