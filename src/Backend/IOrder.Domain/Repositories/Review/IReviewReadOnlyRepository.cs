using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Review;

public interface IReviewReadOnlyRepository
{
    Task<OrderReview?> GetByOrderIdAsync(Guid orderId);
    Task<IList<OrderReview>> GetByStoreIdAsync(Guid storeId, int page, int pageSize);
}
