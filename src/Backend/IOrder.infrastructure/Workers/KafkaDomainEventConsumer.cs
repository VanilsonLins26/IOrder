using System.Text.Json;
using Confluent.Kafka;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IOrder.infrastructure.Workers;

public class KafkaDomainEventConsumer : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly string _topic;
    private readonly ILogger<KafkaDomainEventConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEmailService _emailService;
    private readonly string? _adminEmail;

    public KafkaDomainEventConsumer(
        IConfiguration configuration,
        ILogger<KafkaDomainEventConsumer> logger,
        IServiceScopeFactory scopeFactory,
        IEmailService emailService)
    {
        _topic = configuration["Kafka:Topic"] ?? "domain.events";
        _logger = logger;
        _scopeFactory = scopeFactory;
        _emailService = emailService;
        _adminEmail = configuration["Smtp:AdminEmail"];

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

    internal async Task HandleEventAsync(DomainEventEnvelope envelope, CancellationToken stoppingToken)
    {
        switch (envelope.EventType)
        {
            case "OrderCreatedEvent":
                await HandleOrderCreatedAsync(envelope, stoppingToken);
                break;
            case "OrderStatusChangedEvent":
                await HandleOrderStatusChangedAsync(envelope, stoppingToken);
                break;
            case "StoreCreatedEvent":
                await HandleStoreCreatedAsync(envelope, stoppingToken);
                break;
            default:
                _logger.LogWarning("Unknown event type: {EventType}", envelope.EventType);
                break;
        }
    }

    private async Task HandleOrderCreatedAsync(DomainEventEnvelope envelope, CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "[OrderCreated] Order {OrderId} created by user {UserId} at store {StoreId} — total: {Total}",
            envelope.Data?.OrderId, envelope.Data?.UserId, envelope.Data?.StoreId, envelope.Data?.TotalAmount);

        if (envelope.Data?.StoreId is null) return;

        using var scope = _scopeFactory.CreateScope();
        var storeRepository = scope.ServiceProvider.GetRequiredService<IStoreReadOnlyRepository>();
        var store = await storeRepository.GetByIdAsync(envelope.Data.StoreId.Value);

        if (store?.OwnerEmail is null)
        {
            _logger.LogWarning("Store {StoreId} has no owner email", envelope.Data.StoreId);
            return;
        }

        var subject = "Novo pedido recebido!";
        var body = $"""
            <h2>Novo pedido recebido!</h2>
            <p>Olá!</p>
            <p>Você recebeu um novo pedido no valor de <strong>R$ {envelope.Data.TotalAmount:F2}</strong>.</p>
            <p>Acesse o painel da sua loja para visualizar os detalhes.</p>
            <br/>
            <p>Atenciosamente,<br/>Equipe IOrder</p>
            """;

        await _emailService.SendAsync(store.OwnerEmail, subject, body);
    }

    private async Task HandleOrderStatusChangedAsync(DomainEventEnvelope envelope, CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "[OrderStatusChanged] Order {OrderId} changed from {OldStatus} to {NewStatus}",
            envelope.Data?.OrderId, envelope.Data?.OldStatus, envelope.Data?.NewStatus);

        if (envelope.Data?.OrderId is null) return;

        using var scope = _scopeFactory.CreateScope();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderReadOnlyRepository>();
        var order = await orderRepository.GetByIdAsync(envelope.Data.OrderId.Value);

        if (order?.CustomerEmail is null)
        {
            _logger.LogWarning("Order {OrderId} has no customer email", envelope.Data.OrderId);
            return;
        }

        var oldStatus = envelope.Data?.OldStatus ?? "Desconhecido";
        var newStatus = envelope.Data?.NewStatus ?? "Desconhecido";
        var subject = "Status do pedido atualizado";
        var body = $"""
            <h2>Status do pedido atualizado</h2>
            <p>Olá!</p>
            <p>O status do seu pedido mudou de <strong>{oldStatus}</strong> para <strong>{newStatus}</strong>.</p>
            <p>Acompanhe pelo aplicativo.</p>
            <br/>
            <p>Atenciosamente,<br/>Equipe IOrder</p>
            """;

        await _emailService.SendAsync(order.CustomerEmail, subject, body);
    }

    private async Task HandleStoreCreatedAsync(DomainEventEnvelope envelope, CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "[StoreCreated] Store {StoreId} — {StoreName}",
            envelope.Data?.StoreId, envelope.Data?.StoreName);

        if (_adminEmail is null)
        {
            _logger.LogWarning("Admin email not configured (Smtp:AdminEmail)");
            return;
        }

        var subject = "Nova loja cadastrada";
        var body = $"""
            <h2>Nova loja cadastrada!</h2>
            <p>Uma nova loja foi cadastrada na plataforma:</p>
            <ul>
                <li><strong>Nome:</strong> {envelope.Data?.StoreName}</li>
                <li><strong>ID:</strong> {envelope.Data?.StoreId}</li>
            </ul>
            <br/>
            <p>Atenciosamente,<br/>Equipe IOrder</p>
            """;

        await _emailService.SendAsync(_adminEmail, subject, body);
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}

public class DomainEventEnvelope
{
    public string? EventType { get; set; }
    public DomainEventData? Data { get; set; }
    public DateTime OccurredOn { get; set; }
}

public class DomainEventData
{
    public Guid? OrderId { get; set; }
    public string? UserId { get; set; }
    public Guid? StoreId { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }
    public string? StoreName { get; set; }
}
