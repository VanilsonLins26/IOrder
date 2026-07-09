using System.Text.Json;
using Confluent.Kafka;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Services;
using Microsoft.Extensions.Caching.Distributed;
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
    private readonly IDistributedCache _cache;
    private readonly string? _adminEmail;
    private readonly string? _adminPhone;

    public KafkaDomainEventConsumer(
        IConfiguration configuration,
        ILogger<KafkaDomainEventConsumer> logger,
        IServiceScopeFactory scopeFactory,
        IEmailService emailService,
        IEvolutionApiService evolutionApiService,
        IDistributedCache cache)
    {
        _topic = configuration["Kafka:Topic"] ?? "domain.events";
        _logger = logger;
        _scopeFactory = scopeFactory;
        _emailService = emailService;
        _evolutionApiService = evolutionApiService;
        _cache = cache;
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
            case "PriceChangedEvent":
                await HandlePriceChangedAsync(envelope, stoppingToken);
                break;
            case "NewOrderMessageEvent":
                await HandleNewOrderMessageAsync(envelope, stoppingToken);
                break;
            case "CouponCreatedEvent":
                await HandleCouponCreatedAsync(envelope, stoppingToken);
                break;
            case "PromotionActivatedEvent":
                await HandlePromotionActivatedAsync(envelope, stoppingToken);
                break;
            case "CartAbandonedEvent":
                await HandleCartAbandonedAsync(envelope, stoppingToken);
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

    private async Task HandlePriceChangedAsync(DomainEventEnvelope envelope, CancellationToken stoppingToken)
    {
        var productId = envelope.Data?.ProductId;

        _logger.LogInformation(
            "[PriceChanged] Product {ProductId} — new price: {NewPrice}",
            productId, envelope.Data?.NewPrice);

        if (productId is null) return;

        using var scope = _scopeFactory.CreateScope();
        var productRepository = scope.ServiceProvider.GetRequiredService<IOrder.Domain.Repositories.Product.IProductReadOnlyRepository>();
        var storeRepository = scope.ServiceProvider.GetRequiredService<IStoreReadOnlyRepository>();

        var product = await productRepository.GetByIdAsync(productId.Value);

        if (product is null)
        {
            _logger.LogWarning("Product {ProductId} not found", productId);
            return;
        }

        var store = await storeRepository.GetByIdAsync(product.StoreId);

        if (store?.OwnerEmail is not null)
        {
            var subject = "Preco alterado: " + product.Name;
            var body = $"""
                <h2>Preco do produto alterado</h2>
                <p>Produto: <strong>{product.Name}</strong></p>
                <p>Novo preco: <strong>R$ {envelope.Data?.NewPrice:F2}</strong></p>
                <p>Loja: {store.Name}</p>
                <br/>
                <p>Atenciosamente,<br/>Equipe IOrder</p>
                """;

            await _emailService.SendAsync(store.OwnerEmail, subject, body);
        }

        if (store?.OwnerPhone is not null)
        {
            var whatsappMessage = $"""
                *Preco alterado: {product.Name}*
                
                Novo preco: R$ {envelope.Data?.NewPrice:F2}
                Loja: {store.Name}
                """;

            await _evolutionApiService.SendTextAsync(store.OwnerPhone, whatsappMessage);
        }
    }

    private async Task HandleNewOrderMessageAsync(DomainEventEnvelope envelope, CancellationToken stoppingToken)
    {
        var orderId = envelope.Data?.OrderId;
        var senderUserId = envelope.Data?.SenderUserId;

        _logger.LogInformation(
            "[NewOrderMessage] New message in order {OrderId} from user {SenderUserId}",
            orderId, senderUserId);

        if (orderId is null || senderUserId is null) return;

        var dedupKey = $"notif:dedup:newmessage:{orderId}";
        var alreadyNotified = await _cache.GetStringAsync(dedupKey, stoppingToken);
        if (alreadyNotified is not null)
        {
            _logger.LogInformation("Skipping notification for order {OrderId} (already notified within TTL)", orderId);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderReadOnlyRepository>();
        var storeRepository = scope.ServiceProvider.GetRequiredService<IStoreReadOnlyRepository>();

        var order = await orderRepository.GetByIdAsync(orderId.Value);
        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found", orderId);
            return;
        }

        var orderIdShort = orderId?.ToString("N")[..8].ToUpper();
        var subject = "Novas mensagens no pedido #" + orderIdShort;
        var body = $"""
            <h2>Você tem novas mensagens!</h2>
            <p>O pedido <strong>#{orderIdShort}</strong> recebeu novas mensagens.</p>
            <p>Acesse o aplicativo para visualizar e responder.</p>
            <br/>
            <p>Atenciosamente,<br/>Equipe IOrder</p>
            """;

        if (senderUserId == order.UserId)
        {
            var store = await storeRepository.GetByIdAsync(order.StoreId);
            if (store?.OwnerEmail is not null)
            {
                await _emailService.SendAsync(store.OwnerEmail, subject, body);
            }
            else
            {
                _logger.LogWarning("Store {StoreId} has no owner email", order.StoreId);
            }
        }
        else
        {
            if (order.CustomerEmail is not null)
            {
                await _emailService.SendAsync(order.CustomerEmail, subject, body);
            }
            else
            {
                _logger.LogWarning("Order {OrderId} has no customer email", orderId);
            }
        }

        await _cache.SetStringAsync(dedupKey, "1", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        }, stoppingToken);
    }

    private async Task HandleCouponCreatedAsync(DomainEventEnvelope envelope, CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "[CouponCreated] Coupon {Code} — value: {Value}",
            envelope.Data?.CouponCode, envelope.Data?.DiscountValue);

        if (_adminEmail is not null)
        {
            var discountLabel = envelope.Data?.DiscountType == "Percentage"
                ? $"{envelope.Data.DiscountValue}%"
                : $"R$ {envelope.Data.DiscountValue:F2}";

            var subject = "Novo cupom: " + (envelope.Data?.CouponCode ?? "");
            var body = $"""
                <h2>Novo cupom criado</h2>
                <p>Codigo: <strong>{envelope.Data?.CouponCode}</strong></p>
                <p>Desconto: <strong>{discountLabel}</strong></p>
                <br/>
                <p>Atenciosamente,<br/>Equipe IOrder</p>
                """;

            await _emailService.SendAsync(_adminEmail, subject, body);
        }

        if (_adminPhone is not null)
        {
            var discountLabel = envelope.Data?.DiscountType == "Percentage"
                ? $"{envelope.Data.DiscountValue}%"
                : $"R$ {envelope.Data.DiscountValue:F2}";

            var whatsappMessage = $"""
                *Novo cupom: {envelope.Data?.CouponCode}*
                
                Desconto: {discountLabel}
                """;

            await _evolutionApiService.SendTextAsync(_adminPhone, whatsappMessage);
        }
    }

    private async Task HandlePromotionActivatedAsync(DomainEventEnvelope envelope, CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "[PromotionActivated] Product {ProductName} — promotional price: {Price}",
            envelope.Data?.ProductName, envelope.Data?.PromotionalPrice);

        if (_adminEmail is not null)
        {
            var subject = "Promocao ativada: " + (envelope.Data?.ProductName ?? "");
            var body = $"""
                <h2>Promocao ativada</h2>
                <p>Produto: <strong>{envelope.Data?.ProductName}</strong></p>
                <p>Preco promocional: <strong>R$ {envelope.Data?.PromotionalPrice:F2}</strong></p>
                <br/>
                <p>Atenciosamente,<br/>Equipe IOrder</p>
                """;

            await _emailService.SendAsync(_adminEmail, subject, body);
        }

        if (_adminPhone is not null)
        {
            var whatsappMessage = $"""
                *Promocao: {envelope.Data?.ProductName}*
                
                Preco promocional: R$ {envelope.Data?.PromotionalPrice:F2}
                """;

            await _evolutionApiService.SendTextAsync(_adminPhone, whatsappMessage);
        }
    }

    private async Task HandleCartAbandonedAsync(DomainEventEnvelope envelope, CancellationToken stoppingToken)
    {
        var userId = envelope.Data?.UserId;
        var userEmail = envelope.Data?.UserEmail;

        _logger.LogInformation(
            "[CartAbandoned] User {UserId} abandoned cart",
            userId);

        if (userEmail is null)
        {
            _logger.LogWarning("User {UserId} has no email — cannot send cart abandoned notification", userId);
            return;
        }

        var subject = "Seu carrinho esta esperando!";
        var body = $"""
            <h2>Voce deixou itens no carrinho</h2>
            <p>Identificamos que voce adicionou produtos ao carrinho mas nao finalizou o pedido.</p>
            <p>Acesse o aplicativo para concluir sua compra.</p>
            <br/>
            <p>Atenciosamente,<br/>Equipe IOrder</p>
            """;

        await _emailService.SendAsync(userEmail, subject, body);
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
    public Guid? ProductId { get; set; }
    public decimal? NewPrice { get; set; }
    public string? SenderUserId { get; set; }
    public string? MessageText { get; set; }
    public string? CouponCode { get; set; }
    public decimal? DiscountValue { get; set; }
    public string? DiscountType { get; set; }
    public string? ProductName { get; set; }
    public decimal? PromotionalPrice { get; set; }
    public string? UserEmail { get; set; }
}
