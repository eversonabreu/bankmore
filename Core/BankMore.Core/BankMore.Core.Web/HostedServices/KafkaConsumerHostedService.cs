using BankMore.Core.Infrastructure.Messaging;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BankMore.Core.Web.HostedServices;

internal sealed class KafkaConsumerHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly ILogger<KafkaConsumerHostedService> _logger;

    public KafkaConsumerHostedService(IServiceProvider serviceProvider,
        IConfiguration configuration, ILogger<KafkaConsumerHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = configuration["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();

        // add here others topics
        _consumer.Subscribe([Topics.CurrentAccountMovementTopicName]);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(stoppingToken);
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetKeyedService<IMessageTopicHandler>(result.Topic);

                if (handler != null && result?.Message?.Value != null)
                {
                    await handler.HandleMessageAsync(result.Message.Value);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("The Kafka topic consumption service has been interrupted.");
                return;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"An error occurred during the execution of the Kafka topic consumption. '{exc.Message}'.");
            }
        }
    }

    public override void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        base.Dispose();
    }
}