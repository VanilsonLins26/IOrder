using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Review;
using Moq;

namespace CommomTestUtilities.Repositories;

public class ReviewReadOnlyRepositoryBuilder
{
    private readonly Mock<IReviewReadOnlyRepository> _repository;

    public ReviewReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IReviewReadOnlyRepository>();
    }

    public ReviewReadOnlyRepositoryBuilder GetByOrderIdAsync(OrderReview? review)
    {
        if (review is not null)
        {
            _repository.Setup(repo => repo.GetByOrderIdAsync(review.OrderId)).ReturnsAsync(review);
        }

        return this;
    }

    public ReviewReadOnlyRepositoryBuilder GetByStoreIdAsync(IList<OrderReview> reviews)
    {
        _repository.Setup(repo => repo.GetByStoreIdAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(reviews);

        return this;
    }

    public IReviewReadOnlyRepository Build() => _repository.Object;
}
