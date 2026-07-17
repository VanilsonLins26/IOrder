using Bogus;
using IOrder.Domain.Entities;

namespace CommomTestUtilities.Entities.Profile;

public class UserAddressBuilder
{
    public static UserAddress Build(string userId = "test-user", bool isDefault = false)
    {
        return new Faker<UserAddress>()
            .CustomInstantiator(f => new UserAddress
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = f.Address.StreetName(),
                ZipCode = f.Address.ZipCode(),
                Street = f.Address.StreetName(),
                Number = f.Address.BuildingNumber(),
                Complement = f.Address.SecondaryAddress(),
                Neighborhood = f.Address.County(),
                City = f.Address.City(),
                State = f.Address.StateAbbr(),
                Latitude = f.Address.Latitude(),
                Longitude = f.Address.Longitude(),
                IsDefault = isDefault
            })
            .Generate();
    }
}
