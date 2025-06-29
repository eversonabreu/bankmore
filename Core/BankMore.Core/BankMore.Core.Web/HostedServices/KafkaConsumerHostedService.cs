using BankMore.Core.Infrastructure.Messaging;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BankMore.Core.Web.HostedServices;

internal sealed class KafkaConsumerHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<KafkaConsumerHostedService> _logger;
    private readonly ConsumerConfig _config;
    private IConsumer<Ignore, string> _consumer;

    public KafkaConsumerHostedService(IServiceProvider serviceProvider,
        IConfiguration configuration, ILogger<KafkaConsumerHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = configuration["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = bool.TryParse(configuration["Kafka:EnableAutoCommit"], out bool result) && result
        };
    }

    private static async Task WaitForKafkaAsync(string bootstrapServers)
    {
        const int maxRetries = 10;

        var config = new AdminClientConfig
        {
            BootstrapServers = bootstrapServers
        };

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                using var adminClient = new AdminClientBuilder(config).Build();
                adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                Console.WriteLine("Kafka online");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Attempt {attempt}/{maxRetries}]. Kafka offline: {ex.Message}");

                if (attempt == maxRetries)
                {
                    throw new Exception("Kafka not respond in max attempts.", ex);
                }

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }

    private static async Task EnsureTopicsExistAsync(IEnumerable<string> topicNames, string bootstrapServers)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = bootstrapServers
        }).Build();

        var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
        var existingTopics = metadata.Topics.Select(t => t.Topic).ToHashSet();

        var topicsToCreate = topicNames
            .Where(t => !existingTopics.Contains(t))
            .Distinct()
            .Select(t => new TopicSpecification
            {
                Name = t,
                NumPartitions = 1,
                ReplicationFactor = 1
            })
            .ToList();

        if (topicsToCreate.Count == 0)
        {
            Console.WriteLine("Kafka all topics already exists.");
            return;
        }

        try
        {
            await adminClient.CreateTopicsAsync(topicsToCreate);
            Console.WriteLine($"Kafka. Created topics: {string.Join(", ", topicsToCreate.Select(t => t.Name))}");
        }
        catch (CreateTopicsException ex)
        {
            foreach (var result in ex.Results)
            {
                if (result.Error.Code != ErrorCode.TopicAlreadyExists)
                    Console.WriteLine($"Kafka fail topic create: '{result.Topic}': {result.Error.Reason}");
                else
                    Console.WriteLine($"Kafka topic '{result.Topic}' já existe already exists.");
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // add here others topics
        IEnumerable<string> topics = [Topics.CurrentAccountMovementTopicName,
            Topics.CurrentAccountTransferTopicName, Topics.CurrentAccountTariffingTopicName];

        await WaitForKafkaAsync(_config.BootstrapServers);
        await EnsureTopicsExistAsync(topics, _config.BootstrapServers);
        _consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
        
        _consumer.Subscribe(topics);

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
                Console.WriteLine($"[KAFKA] Error consuming. '{exc.Message}'.");
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