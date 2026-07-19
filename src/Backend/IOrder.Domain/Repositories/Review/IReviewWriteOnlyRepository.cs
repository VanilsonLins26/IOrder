using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Review;

public interface IReviewWriteOnlyRepository
{
    Task AddAsync(OrderReview review);
}
