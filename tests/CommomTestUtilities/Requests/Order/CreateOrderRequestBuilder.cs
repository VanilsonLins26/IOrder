using Bogus;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests.Order;

public class CreateOrderRequestBuilder
{
    public static CreateOrderRequestDto Build()
    {
        return new Faker<CreateOrderRequestDto>("pt_BR")
            .RuleFor(r => r.CustomerNotes, f => f.Lorem.Sentence(10))
            .RuleFor(r => r.DeliveryDate, f => f.Date.Future())
            .RuleFor(r => r.CustomerPhone, f => f.Phone.PhoneNumber("55###########"))
            .Generate();
    }
}
