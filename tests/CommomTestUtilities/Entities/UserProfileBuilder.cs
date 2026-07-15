using Bogus;
using IOrder.Domain.Entities;

namespace CommomTestUtilities.Entities;

public class UserProfileBuilder
{
    public static UserProfile Build(string userId = "auth0|test")
    {
        return new Faker<UserProfile>()
            .RuleFor(c => c.Id, f => f.Random.Guid())
            .RuleFor(c => c.UserId, f => userId)
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(c => c.EmailManuallySet, f => false)
            .RuleFor(c => c.StripeCustomerId, f => f.Random.String2(15))
            .Generate();
    }
}
