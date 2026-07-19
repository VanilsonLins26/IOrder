using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class OrderReview : EntityBase
{
    public Guid OrderId { get; private set; }
    public Guid StoreId { get; private set; }
    public string UserId { get; private set; }
    public string? CourierUserId { get; private set; }
    
    public int StoreRating { get; private set; }
    public int? CourierRating { get; private set; }
    public string? Comment { get; private set; }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    // Navigation properties
    public Order? Order { get; private set; }
    public Store? Store { get; private set; }

    // Required by EF Core
    private OrderReview() { }

    public OrderReview(Guid orderId, Guid storeId, string userId, string? courierUserId, int storeRating, int? courierRating, string? comment)
    {
        OrderId = orderId;
        StoreId = storeId;
        UserId = userId;
        CourierUserId = courierUserId;
        StoreRating = storeRating;
        CourierRating = courierRating;
        Comment = comment;
    }
}
