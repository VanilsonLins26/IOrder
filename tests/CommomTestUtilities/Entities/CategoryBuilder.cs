using Bogus;
using IOrder.Domain.Entities;
using System;

namespace CommomTestUtilities.Entities;

public class CategoryBuilder
{
    public static Category Build(Guid? storeId = null)
    {
        return new Faker<Category>()
            .RuleFor(c => c.Id, f => f.Random.Guid())
            .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
            .RuleFor(c => c.Position, f => f.Random.Int(0, 10))
            .RuleFor(c => c.StoreId, f => storeId ?? f.Random.Guid())
            .RuleFor(c => c.Active, f => true);
    }
}
