using IOrder.Domain.Entities.Enums;
using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class Coupon : EntityBase, IAggregateRoot
{
    public string Code { get; set; } = string.Empty;
    public CouponDiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public decimal? MinPurchaseAmount { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int MaxUsageCount { get; set; }
    public int CurrentUsageCount { get; set; }

    public bool IsExpired() => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;

    public bool IsUsageLimitReached() => MaxUsageCount > 0 && CurrentUsageCount >= MaxUsageCount;

    public bool IsValid() => Active && !IsExpired() && !IsUsageLimitReached();

    public decimal CalculateDiscount(decimal cartTotal)
    {
        if (!IsValid())
            return 0;

        if (MinPurchaseAmount.HasValue && cartTotal < MinPurchaseAmount.Value)
            return 0;

        var discount = DiscountType switch
        {
            CouponDiscountType.Percentage => cartTotal * DiscountValue / 100m,
            CouponDiscountType.FixedAmount => DiscountValue,
            _ => 0
        };

        if (MaxDiscountAmount.HasValue && discount > MaxDiscountAmount.Value)
            discount = MaxDiscountAmount.Value;

        return Math.Min(discount, cartTotal);
    }
}
