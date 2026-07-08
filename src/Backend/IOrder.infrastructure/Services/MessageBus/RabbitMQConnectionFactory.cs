using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace IOrder.infrastructure.Services.MessageBus;

public class RabbitMQConnectionFactory : IDisposable
{
    private readonly IConnection _connection;

    public RabbitMQConnectionFactory(IConfiguration configuration)
    {
        var host = configuration["RabbitMQ:Host"] ?? "localhost";
        var port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672");
        var user = configuration["RabbitMQ:User"] ?? "iorder";
        var pass = configuration["RabbitMQ:Password"] ?? "iorder123";

        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = user,
            Password = pass
        };

        _connection = factory.CreateConnectionAsync()
            .GetAwaiter().GetResult();
    }

    public IChannel CreateChannel()
    {
        var channel = _connection.CreateChannelAsync()
            .GetAwaiter().GetResult();
        channel.QueueDeclareAsync("order-messages", durable: true, exclusive: false, autoDelete: false)
            .GetAwaiter().GetResult();
        return channel;
    }

    public void Dispose()
    {
        _connection?.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}
