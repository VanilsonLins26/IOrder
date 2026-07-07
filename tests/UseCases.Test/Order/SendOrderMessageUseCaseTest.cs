using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Order;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Order.Commands;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.Order;

public class SendOrderMessageUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var order = OrderBuilder.Build();
        var store = StoreBuilder.Build(order.UserId);
        var request = SendOrderMessageRequestBuilder.Build();
        var useCase = CreateUseCase(order, store);

        var response = await useCase.Execute(order.Id, request);

        response.ShouldNotBeNull();
    }

    [Fact]
    public async Task Error_Order_Not_Found()
    {
        var request = SendOrderMessageRequestBuilder.Build();
        var useCase = CreateUseCase(null);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid(), request);

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.ORDER_NOT_FOUND);
    }

    [Fact]
    public async Task Error_Validation_Failed()
    {
        var order = OrderBuilder.Build();
        var store = StoreBuilder.Build(order.UserId);
        var request = SendOrderMessageRequestBuilder.Build();
        request.Message = string.Empty;
        var useCase = CreateUseCase(order, store);

        Func<Task> act = async () => await useCase.Execute(order.Id, request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.ORDER_MESSAGE_EMPTY);
    }

    private static SendOrderMessageUseCase CreateUseCase(
        IOrder.Domain.Entities.Order? order,
        IOrder.Domain.Entities.Store? store = null)
    {
        var writeOnly = new OrderWriteOnlyRepositoryBuilder()
            .GetByIdTracking(order)
            .Build();
        var storeReadOnly = new StoreReadOnlyRepositoryBuilder();
        if (store is not null)
            storeReadOnly.GetByIdAsync(store);
        var loggedUser = LoggedUserBuilder.Build(order?.UserId ?? "test-user-id");
        var uow = UnitOfWorkBuilder.Build();
        var validator = new SendOrderMessageValidator();

        return new SendOrderMessageUseCase(
            writeOnly,
            storeReadOnly.Build(),
            loggedUser,
            uow,
            validator);
    }
}
