using CommomTestUtilities.Requests.Store;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Services;
using IOrder.infrastructure.DataAccess;
using Moq;
using IOrder.infrastructure.Workers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.Notifications;

public class NotificationIntegrationTest : IOrderClassFixture
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly AppDbContext _dbContext;

    public NotificationIntegrationTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task CreateStoreAndOrder_ConsumerHandlesEvents()
    {
        var request = StoreRequestBuilder.Build();
        request.CategoryId = _dbContext.StoreCategories.First().Id;

        var token = "notification-test-" + Random.Shared.NextInt64();
        var response = await DoPost("store", request, token);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var storeRepo = _factory.Services.GetRequiredService<IStoreReadOnlyRepository>();
        var store = await storeRepo.GetByUserIdAsync(token);
        store.ShouldNotBeNull();
        store.OwnerEmail.ShouldNotBeNull();
        store.OwnerPhone.ShouldNotBeNull();

        var consumer = _factory.Services.GetRequiredService<KafkaDomainEventConsumer>();

        var orderId = System.Guid.NewGuid();
        var envelope = new DomainEventEnvelope
        {
            EventType = "OrderCreatedEvent",
            Data = new DomainEventData
            {
                OrderId = orderId,
                StoreId = store.Id,
                TotalAmount = 250.00m
            },
            OccurredOn = System.DateTime.UtcNow
        };

        await consumer.HandleEventAsync(envelope, CancellationToken.None);

        _factory.EmailMock.Verify(
            e => e.SendAsync(store.OwnerEmail!, It.IsAny<string>(), It.IsAny<string>()),
            Moq.Times.Once);
        _factory.EvolutionMock.Verify(
            e => e.SendTextAsync(store.OwnerPhone!, It.IsAny<string>()),
            Moq.Times.Once);
    }
}
