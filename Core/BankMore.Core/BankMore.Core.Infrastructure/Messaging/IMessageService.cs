namespace BankMore.Core.Infrastructure.Messaging;

public interface IMessageService
{
    public Task PublishAsync<TValue>(string topic, TValue value);
}