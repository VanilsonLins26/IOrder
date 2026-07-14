using Bogus;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests.Payment;

public class CreatePaymentRequestBuilder
{
    public static CreatePaymentRequestDto Build(Guid orderId)
    {
        return new Faker<CreatePaymentRequestDto>()
            .CustomInstantiator(f => new CreatePaymentRequestDto
            {
                OrderId = orderId
            });
    }
}
