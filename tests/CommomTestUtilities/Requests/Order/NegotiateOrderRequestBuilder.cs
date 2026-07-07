using Bogus;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests.Order;

public class NegotiateOrderRequestBuilder
{
    public static NegotiateOrderRequestDto Build()
    {
        return new Faker<NegotiateOrderRequestDto>("pt_BR")
            .RuleFor(r => r.ShopkeeperNotes, f => f.Lorem.Sentence(10))
            .RuleFor(r => r.ProposedTotalAmount, f => f.Random.Decimal(10, 500))
            .RuleFor(r => r.ProposedDeliveryDate, f => f.Date.Future())
            .Generate();
    }
}
