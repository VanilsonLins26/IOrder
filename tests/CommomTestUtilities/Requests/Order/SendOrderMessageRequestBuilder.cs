using Bogus;
using IOrder.Communication.Enums;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests.Order;

public class SendOrderMessageRequestBuilder
{
    public static SendOrderMessageRequestDto Build()
    {
        return new Faker<SendOrderMessageRequestDto>("pt_BR")
            .RuleFor(r => r.Message, f => f.Lorem.Sentence(10))
            .RuleFor(r => r.Type, MessageTypeDto.Text)
            .Generate();
    }
}
