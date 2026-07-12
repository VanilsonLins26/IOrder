using Bogus;
using IOrder.Communication.Enums;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests.Payment;

public class CreatePaymentRequestBuilder
{
    public static CreatePaymentRequestDto Build(PaymentMethodDto? method = null)
    {
        var faker = new Faker("pt_BR");
        var selectedMethod = method ?? faker.PickRandom<PaymentMethodDto>();

        var request = new Faker<CreatePaymentRequestDto>("pt_BR")
            .RuleFor(r => r.OrderId, f => f.Random.Guid())
            .RuleFor(r => r.Method, selectedMethod)
            .RuleFor(r => r.PayerEmail, f => f.Internet.Email())
            .RuleFor(r => r.PayerIdentificationType, "CPF")
            .RuleFor(r => r.PayerIdentificationNumber, f => f.Random.ReplaceNumbers("###########"))
            .Generate();

        if (selectedMethod == PaymentMethodDto.CreditCard)
        {
            request.CardToken = faker.Random.AlphaNumeric(36);
            request.Installments = faker.Random.Int(1, 12);
        }

        return request;
    }

    public static CreatePaymentRequestDto BuildPix()
    {
        return Build(PaymentMethodDto.Pix);
    }

    public static CreatePaymentRequestDto BuildCreditCard()
    {
        return Build(PaymentMethodDto.CreditCard);
    }

    public static CreatePaymentRequestDto BuildBoleto()
    {
        return Build(PaymentMethodDto.Boleto);
    }
}
