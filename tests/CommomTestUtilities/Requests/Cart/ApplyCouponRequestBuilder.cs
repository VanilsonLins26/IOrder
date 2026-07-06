using Bogus;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests.Cart;

public class ApplyCouponRequestBuilder
{
    public static ApplyCouponRequestDto Build()
    {
        return new Faker<ApplyCouponRequestDto>()
            .RuleFor(r => r.CouponCode, f => f.Commerce.ProductName()) // Or a random alphanumeric code
            .Generate();
    }
}
