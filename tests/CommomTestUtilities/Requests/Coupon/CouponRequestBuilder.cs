using Bogus;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests.Coupon;

public class CouponRequestBuilder
{
    public static CouponRequestDto Build()
    {
        return new Faker<CouponRequestDto>("pt_BR")
            .RuleFor(r => r.Code, f => { var d = f.Commerce.Department(); return d.ToUpper()[..Math.Min(10, d.Length)]; })
            .RuleFor(r => r.DiscountType, "Percentage")
            .RuleFor(r => r.DiscountValue, 10m)
            .RuleFor(r => r.MaxDiscountAmount, 50m)
            .RuleFor(r => r.MinPurchaseAmount, 20m)
            .RuleFor(r => r.ExpiresAt, DateTime.UtcNow.AddMonths(1))
            .RuleFor(r => r.MaxUsageCount, 100)
            .Generate();
    }
}
