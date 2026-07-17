using Bogus;
using IOrder.Communication.Response;
using IOrder.Application.Services.Payment;

namespace CommomTestUtilities.Responses.Payment;

public class PaymentCardResponseBuilder
{
    public static List<UserCardDto> Build(int count = 2)
    {
        return new Faker<UserCardDto>()
            .RuleFor(c => c.GatewayCardId, f => f.Random.String2(15))
            .RuleFor(c => c.LastFourDigits, f => f.Random.String2(4, "0123456789"))
            .RuleFor(c => c.Brand, f => f.PickRandom("Visa", "Mastercard", "Amex"))
            .RuleFor(c => c.ExpirationMonth, f => f.Random.Int(1, 12))
            .RuleFor(c => c.ExpirationYear, f => f.Random.Int(2025, 2035))
            .Generate(count);
    }
}
