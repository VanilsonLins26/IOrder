using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IOrder.infrastructure.Workers;

[ExcludeFromCodeCoverage]
public class KafkaDomainEventConsumer : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly string _topic;
    private readonly ILogger<KafkaDomainEventConsumer> _logger;

    public KafkaDomainEventConsumer(IConfiguration configuration, ILogger<KafkaDomainEventConsumer> logger)
    {
        _topic = configuration["Kafka:Topic"] ?? "domain.events";
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            GroupId = configuration["Kafka:GroupId"] ?? "domain-events-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);
        _logger.LogInformation("Kafka consumer started for topic: {Topic}", _topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);

                    if (result is null) continue;

                    await ProcessMessageAsync(result.Message, stoppingToken);

                    _consumer.Commit(result);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Kafka message");
                }
            }
        }
        finally
        {
            _consumer.Close();
        }
    }

    private async Task ProcessMessageAsync(Message<string, string> message, CancellationToken stoppingToken)
    {
        try
        {
            var envelope = JsonSerializer.Deserialize<DomainEventEnvelope>(message.Value);

            if (envelope is null)
            {
                _logger.LogWarning("Failed to deserialize message: {Value}", message.Value);
                return;
            }

            _logger.LogInformation(
                "Received domain event: {EventType} | OrderId: {OrderId} | OccurredOn: {OccurredOn}",
                envelope.EventType, envelope.Data?.OrderId, envelope.OccurredOn);

            await HandleEventAsync(envelope, stoppingToken);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid JSON in Kafka message: {Value}", message.Value);
        }
    }

    private Task HandleEventAsync(DomainEventEnvelope envelope, CancellationToken stoppingToken)
    {
        return envelope.EventType switch
        {
            "OrderCreatedEvent" => HandleOrderCreatedAsync(envelope, stoppingToken),
            "OrderStatusChangedEvent" => HandleOrderStatusChangedAsync(envelope, stoppingToken),
            _ => HandleUnknownEventAsync(envelope, stoppingToken)
        };
    }

    private Task HandleOrderCreatedAsync(DomainEventEnvelope envelope, CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "[OrderCreated] Order {OrderId} created by user {UserId} at store {StoreId} — total: {Total}",
            envelope.Data?.OrderId, envelope.Data?.UserId, envelope.Data?.StoreId, envelope.Data?.TotalAmount);

        return Task.CompletedTask;
    }

    private Task HandleOrderStatusChangedAsync(DomainEventEnvelope envelope, CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "[OrderStatusChanged] Order {OrderId} changed from {OldStatus} to {NewStatus}",
            envelope.Data?.OrderId, envelope.Data?.OldStatus, envelope.Data?.NewStatus);

        return Task.CompletedTask;
    }

    private Task HandleUnknownEventAsync(DomainEventEnvelope envelope, CancellationToken stoppingToken)
    {
        _logger.LogWarning("Unknown event type: {EventType}", envelope.EventType);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}

internal class DomainEventEnvelope
{
    public string? EventType { get; set; }
    public DomainEventData? Data { get; set; }
    public DateTime OccurredOn { get; set; }
}

internal class DomainEventData
{
    public Guid OrderId { get; set; }
    public string? UserId { get; set; }
    public Guid StoreId { get; set; }
    public decimal TotalAmount { get; set; }
    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }
}
