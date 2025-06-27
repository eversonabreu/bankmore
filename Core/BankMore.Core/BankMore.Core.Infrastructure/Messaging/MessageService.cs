using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BankMore.Core.Infrastructure.Messaging;

public sealed class MessageService : IMessageService
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<MessageService> _logger;

    public MessageService(IConfiguration config, ILogger<MessageService> logger)
    {
        var bootstrapServers = config["Kafka:BootstrapServers"];
        if (string.IsNullOrWhiteSpace(bootstrapServers))
            throw new InvalidOperationException("Kafka BootstrapServers configuration is missing.");

        var configKafka = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true
        };

        _producer = new ProducerBuilder<string, string>(configKafka).Build();
        _logger = logger;
    }

    public async Task PublishAsync(string topic, string key, string value)
    {
        try
        {
            var msg = new Message<string, string>
            {
                Key = key,
                Value = value
            };

            var deliveryResult = await _producer.ProduceAsync(topic, msg);

            _logger.LogInformation("Mensagem publicada no Kafka - Tópico: {Topic}, Offset: {Offset}", topic, deliveryResult.Offset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem no Kafka.");
            throw;
        }
    }
}