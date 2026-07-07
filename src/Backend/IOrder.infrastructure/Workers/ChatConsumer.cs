using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
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
    private readonly ILogger<ChatConsumer> _logger;

    public ChatConsumer(
        RabbitMQConnectionFactory connectionFactory,
        IServiceScopeFactory scopeFactory,
        IHubContext<ChatHub> hubContext,
        ILogger<ChatConsumer> logger)
    {
        _connectionFactory = connectionFactory;
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
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
