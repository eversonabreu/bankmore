namespace BankMore.Core.Infrastructure.Messaging;

public interface IMessageService
{
    public Task PublishAsync(string topic, string key, string value);
}