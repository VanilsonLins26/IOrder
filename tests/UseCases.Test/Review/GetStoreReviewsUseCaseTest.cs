using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Review.Queries;
using Shouldly;

namespace UseCases.Test.Review;

public class GetStoreReviewsUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var storeId = Guid.NewGuid();
        var reviews = ReviewBuilder.BuildCollection(3, storeId);
        var useCase = CreateUseCase(reviews);

        var response = await useCase.ExecuteAsync(storeId, 1, 10);

        response.ShouldNotBeNull();
        response.Count.ShouldBe(3);
    }

    private GetStoreReviewsUseCase CreateUseCase(IList<IOrder.Domain.Entities.OrderReview> reviews)
    {
        var repository = new ReviewReadOnlyRepositoryBuilder();
        repository.GetByStoreIdAsync(reviews);

        return new GetStoreReviewsUseCase(repository.Build());
    }
}
