using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
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
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ChatConsumer> _logger;

    public ChatConsumer(
        RabbitMQConnectionFactory connectionFactory,
        IHubContext<ChatHub> hubContext,
        IDistributedCache cache,
        ILogger<ChatConsumer> logger)
    {
        _connectionFactory = connectionFactory;
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
                    var dedupKey = $"dedup:chat:{envelope.OrderId}";
                    var isDuplicate = await _cache.GetStringAsync(dedupKey, stoppingToken) is not null;

                    if (isDuplicate)
                    {
                        _logger.LogInformation("Skipping duplicate for order {OrderId}", envelope.OrderId);
                        await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                        return;
                    }

                    await _hubContext.Clients
                        .Group(envelope.OrderId.ToString())
                        .SendAsync("MessageReceived", envelope.Message, stoppingToken);

                    await _hubContext.Clients
                        .Group(envelope.OrderId.ToString())
                        .SendAsync("ConversationUpdated", envelope.Message, stoppingToken);

                    await _cache.SetStringAsync(dedupKey, "1", new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                    }, stoppingToken);
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
}

internal class ChatMessageEnvelope
{
    public Guid OrderId { get; set; }
    public object? Message { get; set; }
    public DateTime Timestamp { get; set; }
}
