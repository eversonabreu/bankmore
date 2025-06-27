namespace BankMore.Core.Infrastructure.Messaging;

public interface IMessageTopicHandler
{
    public Task HandleAsync(string payload);
}