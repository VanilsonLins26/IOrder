using Bogus;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests;

public class AssignCourierRequestBuilder
{
    public static AssignCourierRequestDto Build()
    {
        return new Faker<AssignCourierRequestDto>("pt_BR")
            .RuleFor(r => r.CourierUserId, f => $"auth0|{f.Random.AlphaNumeric(24)}")
            .Generate();
    }
}
