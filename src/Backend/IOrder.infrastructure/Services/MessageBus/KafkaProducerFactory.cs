using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace IOrder.infrastructure.Services.MessageBus;

public class KafkaProducerFactory : IDisposable
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducerFactory(IConfiguration configuration)
    {
        var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public IProducer<string, string> Create() => _producer;

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
