using Bogus;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests;

public class UpdateCourierLocationRequestBuilder
{
    public static UpdateCourierLocationRequestDto Build()
    {
        return new Faker<UpdateCourierLocationRequestDto>("pt_BR")
            .RuleFor(r => r.Latitude, f => f.Address.Latitude())
            .RuleFor(r => r.Longitude, f => f.Address.Longitude())
            .Generate();
    }
}
