using Bogus;
using IOrder.Domain.Entities;
using System;

namespace CommomTestUtilities.Entities;

public class CartBuilder
{
    public static Cart Build(string userId = "test-user-id")
    {
        return new Faker<Cart>()
            .RuleFor(c => c.UserId, f => userId)
            .RuleFor(c => c.CouponCode, f => f.Commerce.ProductName())
            .Generate();
    }
}
