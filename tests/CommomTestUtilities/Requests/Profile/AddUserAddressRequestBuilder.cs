using Bogus;
using IOrder.Communication.Request.Profile;

namespace CommomTestUtilities.Requests.Profile;

public class AddUserAddressRequestBuilder
{
    public static AddUserAddressRequestDto Build()
    {
        return new Faker<AddUserAddressRequestDto>()
            .RuleFor(r => r.Name, f => f.Address.StreetName())
            .RuleFor(r => r.ZipCode, f => f.Address.ZipCode())
            .RuleFor(r => r.Street, f => f.Address.StreetName())
            .RuleFor(r => r.Number, f => f.Address.BuildingNumber())
            .RuleFor(r => r.Complement, f => f.Address.SecondaryAddress())
            .RuleFor(r => r.Neighborhood, f => f.Address.County())
            .RuleFor(r => r.City, f => f.Address.City())
            .RuleFor(r => r.State, f => f.Address.StateAbbr())
            .Generate();
    }
}
