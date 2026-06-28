using Bogus;
using IOrder.Communication.Request;
using System;

namespace CommomTestUtilities.Requests.Category;

public class CategoryRequestBuilder
{
    public static CategoryRequestDto Build()
    {
        return new Faker<CategoryRequestDto>()
            .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
            .RuleFor(c => c.Position, f => f.Random.Int(0, 10));
    }
}
