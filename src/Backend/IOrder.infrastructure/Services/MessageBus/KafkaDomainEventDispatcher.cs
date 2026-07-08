using System.Text.Json;
using Confluent.Kafka;
using IOrder.Domain.SeedWork;
using IOrder.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOrder.infrastructure.Services.MessageBus;

public class KafkaDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly KafkaProducerFactory _producerFactory;
    private readonly string _topic;
    private readonly ILogger<KafkaDomainEventDispatcher> _logger;

    public KafkaDomainEventDispatcher(
        KafkaProducerFactory producerFactory,
        IConfiguration configuration,
        ILogger<KafkaDomainEventDispatcher> logger)
    {
        _producerFactory = producerFactory;
        _topic = configuration["Kafka:Topic"] ?? "domain.events";
        _logger = logger;
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> events)
    {
        var producer = _producerFactory.Create();

        foreach (var domainEvent in events)
        {
            try
            {
                var payload = JsonSerializer.Serialize(new
                {
                    EventType = domainEvent.GetType().Name,
                    Data = domainEvent,
                    OccurredOn = DateTime.UtcNow
                }, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });

                var result = await producer.ProduceAsync(_topic, new Message<string, string>
                {
                    Key = domainEvent.GetType().Name,
                    Value = payload
                });

                _logger.LogInformation(
                    "Published {EventType} to Kafka [topic={Topic}, partition={Partition}, offset={Offset}]",
                    domainEvent.GetType().Name, _topic, result.Partition.Value, result.Offset.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish {EventType} to Kafka", domainEvent.GetType().Name);
            }
        }
    }
}
