using Bogus;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests.Store;

public class UpdateStoreRequestBuilder
{
    public static UpdateStoreRequestDto Build()
    {
        return new Faker<UpdateStoreRequestDto>("pt_BR")
            .RuleFor(store => store.Name, f => f.Company.CompanyName())
            .RuleFor(store => store.About, f => f.Lorem.Paragraph())
            .RuleFor(store => store.ImageUrl, f => f.Image.PicsumUrl());
    }
}
