using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Review;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Review.Commands;
using IOrder.Domain.Entities.Enums;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.Review;

public class CreateReviewUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = CreateReviewRequestBuilder.Build();
        var order = OrderBuilder.Build();
        order.MarkAsDelivered();
        var store = StoreBuilder.Build();
        var useCase = CreateUseCase(order, store: store);

        var response = await useCase.ExecuteAsync(order.Id, request);

        response.ShouldNotBeNull();
        response.Comment.ShouldBe(request.Comment);
        response.StoreRating.ShouldBe(request.StoreRating);
    }

    [Fact]
    public async Task Error_Order_Not_Found()
    {
        var request = CreateReviewRequestBuilder.Build();
        var useCase = CreateUseCase(null);

        Func<Task> act = async () => await useCase.ExecuteAsync(Guid.NewGuid(), request);

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldContain("Pedido não encontrado.");
    }

    [Fact]
    public async Task Error_Different_User()
    {
        var request = CreateReviewRequestBuilder.Build();
        var order = OrderBuilder.Build(userId: "another-user");
        var useCase = CreateUseCase(order);

        Func<Task> act = async () => await useCase.ExecuteAsync(order.Id, request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("Apenas o cliente que fez o pedido pode avaliá-lo.");
    }

    [Fact]
    public async Task Error_Order_Not_Delivered()
    {
        var request = CreateReviewRequestBuilder.Build();
        var order = OrderBuilder.Build();
        var useCase = CreateUseCase(order);

        Func<Task> act = async () => await useCase.ExecuteAsync(order.Id, request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("O pedido deve estar Entregue para ser avaliado.");
    }

    [Fact]
    public async Task Error_Already_Reviewed()
    {
        var request = CreateReviewRequestBuilder.Build();
        var order = OrderBuilder.Build();
        order.MarkAsDelivered();
        var existingReview = ReviewBuilder.Build(orderId: order.Id);
        var useCase = CreateUseCase(order, existingReview);

        Func<Task> act = async () => await useCase.ExecuteAsync(order.Id, request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("Este pedido já foi avaliado.");
    }

    private CreateReviewUseCase CreateUseCase(
        IOrder.Domain.Entities.Order? order,
        IOrder.Domain.Entities.OrderReview? existingReview = null,
        IOrder.Domain.Entities.Store? store = null,
        string loggedUserId = "test-user-id")
    {
        var orderReadOnly = new OrderReadOnlyRepositoryBuilder();
        if (order != null)
            orderReadOnly.GetByIdAsync(order);

        var reviewReadOnly = new ReviewReadOnlyRepositoryBuilder();
        if (existingReview != null)
            reviewReadOnly.GetByOrderIdAsync(existingReview);

        var reviewWriteOnly = new ReviewWriteOnlyRepositoryBuilder();

        var storeWriteOnly = new StoreWriteOnlyRepositoryBuilder();
        if (store != null)
            storeWriteOnly.GetByIdTracking(store);

        var loggedUser = LoggedUserBuilder.Build(loggedUserId);
        var uow = UnitOfWorkBuilder.Build();

        return new CreateReviewUseCase(
            orderReadOnly.Build(),
            reviewReadOnly.Build(),
            reviewWriteOnly.Build(),
            storeWriteOnly.Build(),
            loggedUser,
            uow
        );
    }
}
