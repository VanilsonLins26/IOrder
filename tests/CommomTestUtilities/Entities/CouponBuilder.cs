using Bogus;
using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;

namespace CommomTestUtilities.Entities;

public class CouponBuilder
{
    public static Coupon Build(string? code = null)
    {
        return new Faker<Coupon>("pt_BR")
            .RuleFor(c => c.Id, Guid.CreateVersion7())
            .RuleFor(c => c.Code, f => code ?? f.Commerce.Department())
            .RuleFor(c => c.DiscountType, CouponDiscountType.Percentage)
            .RuleFor(c => c.DiscountValue, 10m)
            .RuleFor(c => c.MaxDiscountAmount, 50m)
            .RuleFor(c => c.MinPurchaseAmount, 20m)
            .RuleFor(c => c.ExpiresAt, DateTime.UtcNow.AddMonths(1))
            .RuleFor(c => c.MaxUsageCount, 100)
            .RuleFor(c => c.CurrentUsageCount, 0);
    }
}
