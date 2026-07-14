using Bogus;
using IOrder.Domain.Entities;

namespace CommomTestUtilities.Entities;

public class PaymentBuilder
{
    public static Payment Build(Guid orderId)
    {
        return new Faker<Payment>()
            .CustomInstantiator(f => new Payment
            {
                OrderId = orderId,
                Amount = f.Random.Decimal(10, 1000),
                Method = IOrder.Domain.Entities.Enums.PaymentMethod.Pix,
                StripePaymentIntentId = f.Random.AlphaNumeric(20)
            });
    }
}
