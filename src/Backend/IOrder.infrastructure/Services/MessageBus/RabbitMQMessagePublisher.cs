using System.Text;
using System.Text.Json;
using IOrder.Domain.Services;
using RabbitMQ.Client;

namespace IOrder.infrastructure.Services.MessageBus;

public class RabbitMQMessagePublisher : IOrderMessagePublisher
{
    private readonly RabbitMQConnectionFactory _connectionFactory;

    public RabbitMQMessagePublisher(RabbitMQConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task PublishMessageAsync(Guid orderId, object message)
    {
        await using var channel = _connectionFactory.CreateChannel();

        var body = new
        {
            OrderId = orderId,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(body);
        var bytes = Encoding.UTF8.GetBytes(json);

        var props = new BasicProperties();

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "order-messages",
            mandatory: false,
            basicProperties: props,
            body: bytes);
    }
}
