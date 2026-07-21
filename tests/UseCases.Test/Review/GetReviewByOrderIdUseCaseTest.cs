using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Review.Queries;
using Shouldly;

namespace UseCases.Test.Review;

public class GetReviewByOrderIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var review = ReviewBuilder.Build();
        var useCase = CreateUseCase(review);

        var response = await useCase.ExecuteAsync(review.OrderId);

        response.ShouldNotBeNull();
        response.Id.ShouldBe(review.Id);
        response.Comment.ShouldBe(review.Comment);
        response.StoreRating.ShouldBe(review.StoreRating);
    }

    [Fact]
    public async Task Success_Not_Found()
    {
        var useCase = CreateUseCase(null);

        var response = await useCase.ExecuteAsync(Guid.NewGuid());

        response.ShouldBeNull();
    }

    private GetReviewByOrderIdUseCase CreateUseCase(IOrder.Domain.Entities.OrderReview? review)
    {
        var repository = new ReviewReadOnlyRepositoryBuilder();
        if (review != null)
            repository.GetByOrderIdAsync(review);

        return new GetReviewByOrderIdUseCase(repository.Build());
    }
}
