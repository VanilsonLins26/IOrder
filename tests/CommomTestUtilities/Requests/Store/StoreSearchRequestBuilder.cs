using Bogus;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests.Store;

public class StoreSearchRequestBuilder
{
    public static StoreSearchRequestDto Build()
    {
        return new Faker<StoreSearchRequestDto>()
            .RuleFor(r => r.PageNumber, f => f.Random.Int(1, 10))
            .RuleFor(r => r.PageSize, f => f.Random.Int(10, 50))
            .RuleFor(r => r.Name, f => f.Company.CompanyName())
            .RuleFor(r => r.CategoryId, f => f.Random.Guid())
            .Generate();
    }
}
