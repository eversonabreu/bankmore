using System.Text.Json;

namespace BankMore.Core.Infrastructure.Messaging;

public interface IMessageTopicHandler
{
    public Task<string> HandleAsync(object body);

    public Task HandleMessageAsync(string payload);
}