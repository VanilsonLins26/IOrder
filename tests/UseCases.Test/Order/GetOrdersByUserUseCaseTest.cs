using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Order.Queries;
using Shouldly;

namespace UseCases.Test.Order;

public class GetOrdersByUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var orders = new List<IOrder.Domain.Entities.Order>
        {
            OrderBuilder.Build(),
            OrderBuilder.Build()
        };
        var useCase = CreateUseCase(orders);

        var response = await useCase.Execute(1, 10);

        response.ShouldNotBeNull();
        response.Items.Count.ShouldBe(2);
        response.TotalCount.ShouldBe(2);
    }

    [Fact]
    public async Task Success_Empty()
    {
        var useCase = CreateUseCase([]);

        var response = await useCase.Execute(1, 10);

        response.Items.Count.ShouldBe(0);
        response.TotalCount.ShouldBe(0);
    }

    private static GetOrdersByUserUseCase CreateUseCase(List<IOrder.Domain.Entities.Order> orders)
    {
        var readOnly = new OrderReadOnlyRepositoryBuilder()
            .GetByUserIdAsync(orders)
            .GetCountByUserIdAsync(orders.Count)
            .Build();
        var loggedUser = LoggedUserBuilder.Build();

        return new GetOrdersByUserUseCase(readOnly, loggedUser);
    }
}
