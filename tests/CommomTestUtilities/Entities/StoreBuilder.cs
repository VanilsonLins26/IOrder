using Bogus;
using IOrder.Domain.Entities;

namespace CommomTestUtilities.Entities;

public class StoreBuilder
{
    public static Store Build(string userId = "test-user-id")
    {
        return new Faker<Store>("pt_BR")
            .RuleFor(s => s.Name, f => f.Company.CompanyName())
            .RuleFor(s => s.About, f => f.Lorem.Paragraph())
            .RuleFor(s => s.ImageUrl, f => f.Image.PicsumUrl())
            .RuleFor(s => s.UserId, _ => userId)
            .RuleFor(s => s.CategoryId, f => f.Random.Guid())
            .RuleFor(s => s.Address, f => new Address {
                ZipCode = f.Address.ZipCode(),
                Street = f.Address.StreetName(),
                Number = f.Address.BuildingNumber(),
                Neighborhood = f.Address.County(),
                City = f.Address.City(),
                State = f.Address.StateAbbr()
            })
            .RuleFor(s => s.OpeningHours, f => [])
            .Generate();
    }
}
