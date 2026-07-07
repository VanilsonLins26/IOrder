using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Order.Queries;
using Shouldly;

namespace UseCases.Test.Order;

public class GetOrdersByStoreUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var storeId = Guid.NewGuid();
        var orders = new List<IOrder.Domain.Entities.Order>
        {
            OrderBuilder.Build(storeId),
            OrderBuilder.Build(storeId)
        };
        var useCase = CreateUseCase(storeId, orders);

        var response = await useCase.Execute(1, 10);

        response.ShouldNotBeNull();
        response.Items.Count.ShouldBe(2);
    }

    private static GetOrdersByStoreUseCase CreateUseCase(Guid storeId, List<IOrder.Domain.Entities.Order> orders)
    {
        var readOnly = new OrderReadOnlyRepositoryBuilder()
            .GetByStoreIdAsync(orders)
            .GetCountByStoreIdAsync(orders.Count)
            .Build();
        var permissionService = StorePermissionServiceBuilder.Build(storeId);

        return new GetOrdersByStoreUseCase(readOnly, permissionService);
    }
}
