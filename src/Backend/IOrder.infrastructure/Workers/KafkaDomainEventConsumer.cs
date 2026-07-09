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
    private readonly IEvolutionApiService _evolutionApiService;
    private readonly string? _adminEmail;
    private readonly string? _adminPhone;

    public KafkaDomainEventConsumer(
        IConfiguration configuration,
        ILogger<KafkaDomainEventConsumer> logger,
        IServiceScopeFactory scopeFactory,
        IEmailService emailService,
        IEvolutionApiService evolutionApiService)
    {
        _topic = configuration["Kafka:Topic"] ?? "domain.events";
        _logger = logger;
        _scopeFactory = scopeFactory;
        _emailService = emailService;
        _evolutionApiService = evolutionApiService;
        _adminEmail = configuration["Smtp:AdminEmail"];
        _adminPhone = configuration["EvolutionApi:AdminNumber"];

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
        }
        else
        {
            var orderIdShort = envelope.Data?.OrderId?.ToString("N")[..8].ToUpper();
            var subject = "Novo pedido #" + orderIdShort;
            var body = $"""
                <h2>Novo pedido recebido!</h2>
                <p>Pedido <strong>#{orderIdShort}</strong></p>
                <p>Valor: <strong>R$ {envelope.Data.TotalAmount:F2}</strong></p>
                <p>Acesse o painel da sua loja para confirmar ou negociar.</p>
                <br/>
                <p>Atenciosamente,<br/>Equipe IOrder</p>
                """;

            await _emailService.SendAsync(store.OwnerEmail, subject, body);
        }

        if (store?.OwnerPhone is not null)
        {
            var orderIdShort = envelope.Data?.OrderId?.ToString("N")[..8].ToUpper();
            var whatsappMessage = $"""
                *Novo pedido #{orderIdShort}*
                
                Valor: R$ {envelope.Data.TotalAmount:F2}
                
                Acesse o painel da sua loja para confirmar ou negociar o pedido.
                """;

            await _evolutionApiService.SendTextAsync(store.OwnerPhone, whatsappMessage);
        }
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
        }
        else
        {
            var orderIdShort = envelope.Data?.OrderId?.ToString("N")[..8].ToUpper();
            var oldStatus = envelope.Data?.OldStatus ?? "Desconhecido";
            var newStatus = envelope.Data?.NewStatus ?? "Desconhecido";
            var subject = "Pedido #" + orderIdShort + " - " + newStatus;
            var body = $"""
                <h2>Status do pedido atualizado</h2>
                <p>Pedido <strong>#{orderIdShort}</strong></p>
                <p>Status: <strong>{oldStatus}</strong> > <strong>{newStatus}</strong></p>
                <p>Acompanhe pelo aplicativo para mais detalhes.</p>
                <br/>
                <p>Atenciosamente,<br/>Equipe IOrder</p>
                """;

            await _emailService.SendAsync(order.CustomerEmail, subject, body);
        }

        if (order?.CustomerPhone is not null)
        {
            var orderIdShort = envelope.Data?.OrderId?.ToString("N")[..8].ToUpper();
            var oldStatus = envelope.Data?.OldStatus ?? "Desconhecido";
            var newStatus = envelope.Data?.NewStatus ?? "Desconhecido";
            var whatsappMessage = $"""
                *Pedido #{orderIdShort} - {newStatus}*
                
                Status atualizado: {oldStatus} > {newStatus}
                
                Acompanhe pelo aplicativo para mais detalhes.
                """;

            await _evolutionApiService.SendTextAsync(order.CustomerPhone, whatsappMessage);
        }
    }

    private async Task HandleStoreCreatedAsync(DomainEventEnvelope envelope, CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "[StoreCreated] Store {StoreId} — {StoreName}",
            envelope.Data?.StoreId, envelope.Data?.StoreName);

        if (_adminEmail is not null)
        {
            var subject = "Nova loja: " + (envelope.Data?.StoreName ?? "");
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

        if (_adminPhone is not null)
        {
            var whatsappMessage = $"""
                *Nova loja cadastrada*
                
                Loja: {envelope.Data?.StoreName}
                ID: {envelope.Data?.StoreId}
                """;

            await _evolutionApiService.SendTextAsync(_adminPhone, whatsappMessage);
        }
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
