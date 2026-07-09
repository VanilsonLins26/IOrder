using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Services;
using IOrder.infrastructure.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace UseCases.Test.Notifications;

public class KafkaDomainEventConsumerTest
{
    [Fact]
    public async Task Success_OrderCreated_SendsEmailToStoreOwner()
    {
        var store = StoreBuilder.Build();
        var envelope = new DomainEventEnvelope
        {
            EventType = "OrderCreatedEvent",
            Data = new DomainEventData
            {
                OrderId = Guid.NewGuid(),
                StoreId = store.Id,
                TotalAmount = 150.00m
            },
            OccurredOn = DateTime.UtcNow
        };

        var emailMock = new Mock<IEmailService>();
        var evolutionMock = new Mock<IEvolutionApiService>();
        var storeRepo = new StoreReadOnlyRepositoryBuilder().GetByIdAsync(store).Build();
        var scopeFactory = CreateScopeFactory<IStoreReadOnlyRepository>(storeRepo);
        var consumer = BuildConsumer(emailMock, evolutionMock, scopeFactory);

        await consumer.HandleEventAsync(envelope, CancellationToken.None);

        emailMock.Verify(
            e => e.SendAsync(store.OwnerEmail!, It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
        evolutionMock.Verify(
            e => e.SendTextAsync(store.OwnerPhone!, It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Success_OrderStatusChanged_SendsEmailAndWhatsAppToCustomer()
    {
        var order = OrderBuilder.Build();
        var envelope = new DomainEventEnvelope
        {
            EventType = "OrderStatusChangedEvent",
            Data = new DomainEventData
            {
                OrderId = order.Id,
                OldStatus = "Pending",
                NewStatus = "Confirmed"
            },
            OccurredOn = DateTime.UtcNow
        };

        var emailMock = new Mock<IEmailService>();
        var evolutionMock = new Mock<IEvolutionApiService>();
        var orderRepo = new OrderReadOnlyRepositoryBuilder().GetByIdAsync(order).Build();
        var scopeFactory = CreateScopeFactory<IOrderReadOnlyRepository>(orderRepo);
        var consumer = BuildConsumer(emailMock, evolutionMock, scopeFactory);

        await consumer.HandleEventAsync(envelope, CancellationToken.None);

        emailMock.Verify(
            e => e.SendAsync(order.CustomerEmail!, It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
        evolutionMock.Verify(
            e => e.SendTextAsync(order.CustomerPhone!, It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Success_StoreCreated_SendsEmailAndWhatsAppToAdmin()
    {
        var envelope = new DomainEventEnvelope
        {
            EventType = "StoreCreatedEvent",
            Data = new DomainEventData
            {
                StoreId = Guid.NewGuid(),
                StoreName = "Nova Loja"
            },
            OccurredOn = DateTime.UtcNow
        };

        var emailMock = new Mock<IEmailService>();
        var evolutionMock = new Mock<IEvolutionApiService>();
        var scopeFactory = Mock.Of<IServiceScopeFactory>();
        var consumer = BuildConsumer(emailMock, evolutionMock, scopeFactory, adminEmail: "admin@iorder.com", adminPhone: "5585986749331");

        await consumer.HandleEventAsync(envelope, CancellationToken.None);

        emailMock.Verify(
            e => e.SendAsync("admin@iorder.com", It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
        evolutionMock.Verify(
            e => e.SendTextAsync("5585986749331", It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Error_OrderCreated_NullStoreId_DoesNotSendEmail()
    {
        var envelope = new DomainEventEnvelope
        {
            EventType = "OrderCreatedEvent",
            Data = new DomainEventData { OrderId = Guid.NewGuid(), TotalAmount = 100m },
            OccurredOn = DateTime.UtcNow
        };

        var emailMock = new Mock<IEmailService>();
        var evolutionMock = new Mock<IEvolutionApiService>();
        var consumer = BuildConsumer(emailMock, evolutionMock);

        await consumer.HandleEventAsync(envelope, CancellationToken.None);

        emailMock.Verify(
            e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        evolutionMock.Verify(
            e => e.SendTextAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Error_OrderCreated_StoreWithoutOwnerEmail_DoesNotSendEmail()
    {
        var store = StoreBuilder.Build();
        store.OwnerEmail = null;
        store.OwnerPhone = null;

        var envelope = new DomainEventEnvelope
        {
            EventType = "OrderCreatedEvent",
            Data = new DomainEventData { OrderId = Guid.NewGuid(), StoreId = store.Id, TotalAmount = 100m },
            OccurredOn = DateTime.UtcNow
        };

        var emailMock = new Mock<IEmailService>();
        var evolutionMock = new Mock<IEvolutionApiService>();
        var storeRepo = new StoreReadOnlyRepositoryBuilder().GetByIdAsync(store).Build();
        var scopeFactory = CreateScopeFactory<IStoreReadOnlyRepository>(storeRepo);
        var consumer = BuildConsumer(emailMock, evolutionMock, scopeFactory);

        await consumer.HandleEventAsync(envelope, CancellationToken.None);

        emailMock.Verify(
            e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        evolutionMock.Verify(
            e => e.SendTextAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Error_OrderStatusChanged_NullOrderId_DoesNotSendEmail()
    {
        var envelope = new DomainEventEnvelope
        {
            EventType = "OrderStatusChangedEvent",
            Data = new DomainEventData { OldStatus = "A", NewStatus = "B" },
            OccurredOn = DateTime.UtcNow
        };

        var emailMock = new Mock<IEmailService>();
        var evolutionMock = new Mock<IEvolutionApiService>();
        var consumer = BuildConsumer(emailMock, evolutionMock);

        await consumer.HandleEventAsync(envelope, CancellationToken.None);

        emailMock.Verify(
            e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        evolutionMock.Verify(
            e => e.SendTextAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Error_OrderStatusChanged_OrderWithoutCustomerEmail_DoesNotSendEmail()
    {
        var order = OrderBuilder.Build();
        order.CustomerEmail = null;
        order.CustomerPhone = null;

        var envelope = new DomainEventEnvelope
        {
            EventType = "OrderStatusChangedEvent",
            Data = new DomainEventData { OrderId = order.Id, OldStatus = "A", NewStatus = "B" },
            OccurredOn = DateTime.UtcNow
        };

        var emailMock = new Mock<IEmailService>();
        var evolutionMock = new Mock<IEvolutionApiService>();
        var orderRepo = new OrderReadOnlyRepositoryBuilder().GetByIdAsync(order).Build();
        var scopeFactory = CreateScopeFactory<IOrderReadOnlyRepository>(orderRepo);
        var consumer = BuildConsumer(emailMock, evolutionMock, scopeFactory);

        await consumer.HandleEventAsync(envelope, CancellationToken.None);

        emailMock.Verify(
            e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        evolutionMock.Verify(
            e => e.SendTextAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Error_StoreCreated_AdminNotConfigured_DoesNotSendEmailOrWhatsApp()
    {
        var envelope = new DomainEventEnvelope
        {
            EventType = "StoreCreatedEvent",
            Data = new DomainEventData { StoreId = Guid.NewGuid(), StoreName = "Loja" },
            OccurredOn = DateTime.UtcNow
        };

        var emailMock = new Mock<IEmailService>();
        var evolutionMock = new Mock<IEvolutionApiService>();
        var consumer = BuildConsumer(emailMock, evolutionMock, adminEmail: null, adminPhone: null);

        await consumer.HandleEventAsync(envelope, CancellationToken.None);

        emailMock.Verify(
            e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        evolutionMock.Verify(
            e => e.SendTextAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Error_UnknownEventType_DoesNotSendEmailOrWhatsApp()
    {
        var envelope = new DomainEventEnvelope
        {
            EventType = "UnknownEvent",
            Data = new DomainEventData(),
            OccurredOn = DateTime.UtcNow
        };

        var emailMock = new Mock<IEmailService>();
        var evolutionMock = new Mock<IEvolutionApiService>();
        var consumer = BuildConsumer(emailMock, evolutionMock);

        await consumer.HandleEventAsync(envelope, CancellationToken.None);

        emailMock.Verify(
            e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        evolutionMock.Verify(
            e => e.SendTextAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    private static KafkaDomainEventConsumer BuildConsumer(
        Mock<IEmailService>? emailMock = null,
        Mock<IEvolutionApiService>? evolutionMock = null,
        IServiceScopeFactory? scopeFactory = null,
        string? adminEmail = "admin@test.com",
        string? adminPhone = "5585000000000")
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Smtp:AdminEmail"]).Returns(adminEmail);
        configMock.Setup(c => c["EvolutionApi:AdminNumber"]).Returns(adminPhone);

        var logger = Mock.Of<ILogger<KafkaDomainEventConsumer>>();
        var email = emailMock?.Object ?? Mock.Of<IEmailService>();
        var evolution = evolutionMock?.Object ?? Mock.Of<IEvolutionApiService>();
        var scope = scopeFactory ?? Mock.Of<IServiceScopeFactory>();

        return new KafkaDomainEventConsumer(configMock.Object, logger, scope, email, evolution);
    }

    private static IServiceScopeFactory CreateScopeFactory<T>(T service) where T : class
    {
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(T)))
            .Returns(service);

        var scopeMock = new Mock<IServiceScope>();
        scopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);

        var factoryMock = new Mock<IServiceScopeFactory>();
        factoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);

        return factoryMock.Object;
    }
}
