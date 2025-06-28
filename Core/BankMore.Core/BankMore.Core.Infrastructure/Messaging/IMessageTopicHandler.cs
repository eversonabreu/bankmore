namespace BankMore.Core.Infrastructure.Messaging;

public interface IMessageTopicHandler
{
    public Task<string> HandleAsync(string payload);
}