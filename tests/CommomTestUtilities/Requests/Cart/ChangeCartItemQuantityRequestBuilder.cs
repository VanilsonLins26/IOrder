using Bogus;
using IOrder.Communication.Request;
using System;

namespace CommomTestUtilities.Requests.Cart;

public class ChangeCartItemQuantityRequestBuilder
{
    public static ChangeCartItemQuantityRequestDto Build()
    {
        return new Faker<ChangeCartItemQuantityRequestDto>()
            .RuleFor(r => r.CartItemId, f => Guid.NewGuid())
            .RuleFor(r => r.NewQuantity, f => f.Random.Int(min: 1, max: 100))
            .Generate();
    }
}
