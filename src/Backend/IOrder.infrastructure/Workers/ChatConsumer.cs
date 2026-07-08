using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using IOrder.infrastructure.Hubs;
using IOrder.infrastructure.Services.MessageBus;

namespace IOrder.infrastructure.Workers;

public class ChatConsumer : BackgroundService
{
    private readonly RabbitMQConnectionFactory _connectionFactory;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ChatConsumer> _logger;

    public ChatConsumer(
        RabbitMQConnectionFactory connectionFactory,
        IServiceScopeFactory scopeFactory,
        IHubContext<ChatHub> hubContext,
        IDistributedCache cache,
        ILogger<ChatConsumer> logger)
    {
        _connectionFactory = connectionFactory;
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _connectionFactory.CreateChannel();

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var envelope = JsonSerializer.Deserialize<ChatMessageEnvelope>(json);

                if (envelope is not null)
                {
                    await _hubContext.Clients
                        .Group(envelope.OrderId.ToString())
                        .SendAsync("MessageReceived", envelope.Message, stoppingToken);

                    await TrySetDedupAsync(envelope.OrderId, stoppingToken);
                }

                await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                await channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };

        await channel.BasicConsumeAsync("order-messages", autoAck: false, consumer: consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task TrySetDedupAsync(Guid orderId, CancellationToken stoppingToken)
    {
        var dedupKey = $"dedup:chat:{orderId}";
        var existing = await _cache.GetStringAsync(dedupKey, stoppingToken);

        if (existing is null)
        {
            await _cache.SetStringAsync(dedupKey, "1", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            }, stoppingToken);

            _logger.LogInformation("New dedup window started for order {OrderId}", orderId);
        }
    }
}

internal class ChatMessageEnvelope
{
    public Guid OrderId { get; set; }
    public object? Message { get; set; }
    public DateTime Timestamp { get; set; }
}
