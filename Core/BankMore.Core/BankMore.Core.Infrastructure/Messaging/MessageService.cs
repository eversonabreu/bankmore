using System.Text.Json;
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
        var configKafka = new ProducerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"],
            Acks = Acks.All,
            EnableIdempotence = true
        };

        _producer = new ProducerBuilder<string, string>(configKafka).Build();
        _logger = logger;
    }

    public async Task PublishAsync<TValue>(string topic, TValue value)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);

            var msg = new Message<string, string>
            {
                Value = json
            };

            var deliveryResult = await _producer.ProduceAsync(topic, msg);

            _logger.LogInformation("Kafka message published - Topic: {Topic}, Offset: {Offset}", topic, deliveryResult.Offset);
        }
        catch (ProduceException<string, string> pexc)
        {
            _logger.LogError(pexc, "Kafka message publish error.");
            Console.WriteLine($"Error Kafka producer message: '{pexc.Message}'.");
            throw;
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Kafka message publish error default.");
            Console.WriteLine($"Error Kafka producer default message: '{exc.Message}'.");
            throw;
        }
    }
}