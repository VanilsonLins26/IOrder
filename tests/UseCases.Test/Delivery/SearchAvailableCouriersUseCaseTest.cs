using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Repositories.Delivery;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Delivery.Queries;
using IOrder.Domain.Entities;

using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Repositories.Delivery;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using NetTopologySuite.Geometries;

namespace UseCases.Test.Delivery;

public class SearchAvailableCouriersUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var order = OrderBuilder.Build();
        order.Store = StoreBuilder.Build();
        order.Store.DeliveryPartner = DeliveryPartner.App;
        order.Store.Location = new Point(10, 10);
        
        var couriers = new List<AvailableCourier>
        {
            new AvailableCourier { CourierUserId = "courier-1", DistanceKm = 2.5, LastLocationAt = DateTime.UtcNow }
        };

        var useCase = CreateUseCase(order, couriers);

        var response = await useCase.Execute(order.Id);

        response.ShouldNotBeNull();
        response.Count.ShouldBe(1);
        response.First().CourierUserId.ShouldBe("courier-1");
    }

    [Fact]
    public async Task Error_OrderNotFound()
    {
        var useCase = CreateUseCase(null, new List<AvailableCourier>());

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldContain(ResourceMessagesException.ORDER_NOT_FOUND);
    }

    [Fact]
    public async Task Error_StoreWithoutLocation()
    {
        var order = OrderBuilder.Build();
        order.Store = StoreBuilder.Build();
        order.Store.Location = null;
        var useCase = CreateUseCase(order, new List<AvailableCourier>());

        Func<Task> act = async () => await useCase.Execute(order.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("Loja sem localização configurada.");
    }

    [Fact]
    public async Task Error_StoreDeliveryPartnerNotApp()
    {
        var order = OrderBuilder.Build();
        order.Store = StoreBuilder.Build();
        order.Store.DeliveryPartner = DeliveryPartner.Own;
        order.Store.Location = new Point(10, 10);
        var useCase = CreateUseCase(order, new List<AvailableCourier>());

        Func<Task> act = async () => await useCase.Execute(order.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("Busca de entregadores disponível apenas para entregas do app.");
    }

    private SearchAvailableCouriersUseCase CreateUseCase(IOrder.Domain.Entities.Order? order, List<AvailableCourier> couriers)
    {
        var loggedUser = LoggedUserBuilder.Build("store-owner");
        var storePermission = StorePermissionServiceBuilder.Build(order?.StoreId);
        
        var orderReadOnly = new OrderReadOnlyRepositoryBuilder()
            .GetByIdAsync(order)
            .Build();

        var locationReadOnly = new CourierLocationReadOnlyRepositoryBuilder()
            .GetAvailableCouriersAsync(couriers)
            .Build();

        return new SearchAvailableCouriersUseCase(
            loggedUser,
            storePermission,
            orderReadOnly,
            locationReadOnly);
    }
}
