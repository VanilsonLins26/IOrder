using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Order.Queries;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.Order;

public class GetOrderByIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var order = OrderBuilder.Build();
        var useCase = CreateUseCase(order);

        var response = await useCase.Execute(order.Id);

        response.ShouldNotBeNull();
        response.Id.ShouldBe(order.Id);
    }

    [Fact]
    public async Task Error_Not_Found()
    {
        var useCase = CreateUseCase(null);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.ORDER_NOT_FOUND);
    }

    private static GetOrderByIdUseCase CreateUseCase(IOrder.Domain.Entities.Order? order)
    {
        var readOnly = new OrderReadOnlyRepositoryBuilder()
            .GetByIdAsync(order)
            .Build();
        var loggedUser = LoggedUserBuilder.Build();

        return new GetOrderByIdUseCase(readOnly, loggedUser);
    }
}
