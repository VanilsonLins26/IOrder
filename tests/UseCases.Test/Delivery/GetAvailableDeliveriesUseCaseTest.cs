using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Repositories.Delivery;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Delivery.Queries;
using IOrder.Domain.Entities;

using Shouldly;
using NetTopologySuite.Geometries;

namespace UseCases.Test.Delivery;

public class GetAvailableDeliveriesUseCaseTest
{
    [Fact]
    public async Task Success_WithLocation()
    {
        var courierId = "test-courier";
        var location = CourierLocationBuilder.Build(courierId);
        var orders = new List<IOrder.Domain.Entities.Order> { OrderBuilder.Build() };
        var useCase = CreateUseCase(courierId, location, orders);

        var response = await useCase.ExecuteAsync();

        response.ShouldNotBeNull();
        response.Items.ShouldNotBeNull();
        response.Items.Count.ShouldBe(1);
        response.Items.First().OrderId.ShouldBe(orders[0].Id);
    }

    [Fact]
    public async Task Success_WithoutLocation()
    {
        var courierId = "test-courier";
        var orders = new List<IOrder.Domain.Entities.Order> { OrderBuilder.Build() };
        var useCase = CreateUseCase(courierId, null, orders);

        var response = await useCase.ExecuteAsync();

        response.ShouldNotBeNull();
        response.Items.ShouldNotBeNull();
        response.Items.Count.ShouldBe(1);
    }

    private GetAvailableDeliveriesUseCase CreateUseCase(string courierId, CourierLocation? location, List<IOrder.Domain.Entities.Order> orders)
    {
        var loggedUser = LoggedUserBuilder.Build(courierId);
        var locationReadOnly = new CourierLocationReadOnlyRepositoryBuilder()
            .GetByCourierUserIdAsync(location)
            .Build();
        
        var orderReadOnly = new OrderReadOnlyRepositoryBuilder()
            .GetAvailableForDeliveryAsync(orders)
            .Build();

        return new GetAvailableDeliveriesUseCase(
            loggedUser,
            orderReadOnly,
            locationReadOnly);
    }
}
