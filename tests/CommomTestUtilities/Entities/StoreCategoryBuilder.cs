using Bogus;
using IOrder.Domain.Entities;
using System;

namespace CommomTestUtilities.Entities;

public class StoreCategoryBuilder
{
    public static StoreCategory Build()
    {
        return new Faker<StoreCategory>()
            .RuleFor(c => c.Id, Guid.NewGuid())
            .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
            .RuleFor(c => c.Active, true)
            .Generate();
    }
}
