using Bogus;
using IOrder.Domain.Entities;
using System;

namespace CommomTestUtilities.Entities;

public class CartItemBuilder
{
    public static CartItem Build(Guid? productId = null)
    {
        return new Faker<CartItem>()
            .RuleFor(c => c.Id, f => Guid.NewGuid())
            .RuleFor(c => c.ProductId, f => productId ?? Guid.NewGuid())
            .RuleFor(c => c.Quantity, f => f.Random.Int(1, 10))
            .RuleFor(c => c.UnitPrice, f => f.Finance.Amount(5, 500))
            .Generate();
    }
}
