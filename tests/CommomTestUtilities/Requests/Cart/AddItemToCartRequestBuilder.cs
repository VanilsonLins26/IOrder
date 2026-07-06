using Bogus;
using IOrder.Communication.Request;
using System;

namespace CommomTestUtilities.Requests.Cart;

public class AddItemToCartRequestBuilder
{
    public static AddItemToCartRequestDto Build()
    {
        return new Faker<AddItemToCartRequestDto>()
            .RuleFor(r => r.ProductId, f => Guid.NewGuid())
            .RuleFor(r => r.Quantity, f => f.Random.Int(min: 1, max: 10))
            .RuleFor(r => r.Customize, f => f.Lorem.Sentence())
            .Generate();
    }
}
