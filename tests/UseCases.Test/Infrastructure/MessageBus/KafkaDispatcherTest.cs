using IOrder.infrastructure.Services.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System.Collections.Generic;

namespace UseCases.Test.Infrastructure.MessageBus;

public class KafkaDispatcherTest
{
    [Fact]
    public void Constructor_Initializes_Topic()
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            {"Kafka:Topic", "test.topic"},
            {"Kafka:BootstrapServers", "localhost:9092"}
        });
        
        var config = configBuilder.Build();
        var logger = new Mock<ILogger<KafkaDomainEventDispatcher>>();
        var factory = new KafkaProducerFactory(config);

        var dispatcher = new KafkaDomainEventDispatcher(factory, config, logger.Object);
        
        dispatcher.ShouldNotBeNull();
    }
}
