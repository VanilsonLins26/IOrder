using Bogus;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests.Profile;

public class UserProfileRequestBuilder
{
    public static UserProfileRequestDto Build()
    {
        return new Faker<UserProfileRequestDto>()
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
            .Generate();
    }
}
