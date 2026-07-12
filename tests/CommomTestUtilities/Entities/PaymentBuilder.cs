using Bogus;
using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;

namespace CommomTestUtilities.Entities;

public class PaymentBuilder
{
    public static Payment Build(Guid? orderId = null, PaymentStatus? status = null)
    {
        var faker = new Faker("pt_BR");
        var payment = new Payment
        {
            OrderId = orderId ?? faker.Random.Guid(),
            Amount = faker.Finance.Amount(10, 500),
            CreatedAt = DateTime.UtcNow
        };

        var method = faker.PickRandom<PaymentMethod>();
        switch (method)
        {
            case PaymentMethod.Pix:
                payment.SetPixPayment(
                    faker.Random.AlphaNumeric(12),
                    faker.Image.PicsumUrl(),
                    faker.Random.AlphaNumeric(60));
                break;
            case PaymentMethod.CreditCard:
                payment.SetCardPayment(
                    faker.Random.AlphaNumeric(12),
                    faker.Random.ReplaceNumbers("####"),
                    faker.Random.Int(1, 12),
                    faker.Finance.Amount(10, 100).ToString("F2"));
                break;
            case PaymentMethod.Boleto:
                payment.SetBoletoPayment(
                    faker.Random.AlphaNumeric(12),
                    faker.Internet.Url(),
                    faker.Random.AlphaNumeric(48));
                break;
        }

        if (status.HasValue)
        {
            switch (status.Value)
            {
                case PaymentStatus.Approved:
                    payment.Approve();
                    break;
                case PaymentStatus.Rejected:
                    payment.Reject();
                    break;
                case PaymentStatus.Refunded:
                    payment.Refund();
                    break;
                case PaymentStatus.Cancelled:
                    payment.Cancel();
                    break;
            }
        }

        return payment;
    }

    public static Payment BuildPix(Guid? orderId = null)
    {
        var faker = new Faker("pt_BR");
        var payment = new Payment
        {
            OrderId = orderId ?? faker.Random.Guid(),
            Amount = faker.Finance.Amount(10, 500),
            CreatedAt = DateTime.UtcNow
        };
        payment.SetPixPayment(
            faker.Random.AlphaNumeric(12),
            faker.Image.PicsumUrl(),
            faker.Random.AlphaNumeric(60));
        return payment;
    }

    public static Payment BuildCreditCard(Guid? orderId = null)
    {
        var faker = new Faker("pt_BR");
        var payment = new Payment
        {
            OrderId = orderId ?? faker.Random.Guid(),
            Amount = faker.Finance.Amount(10, 500),
            CreatedAt = DateTime.UtcNow
        };
        payment.SetCardPayment(
            faker.Random.AlphaNumeric(12),
            faker.Random.ReplaceNumbers("####"),
            faker.Random.Int(1, 12),
            faker.Finance.Amount(10, 100).ToString("F2"));
        return payment;
    }

    public static Payment BuildBoleto(Guid? orderId = null)
    {
        var faker = new Faker("pt_BR");
        var payment = new Payment
        {
            OrderId = orderId ?? faker.Random.Guid(),
            Amount = faker.Finance.Amount(10, 500),
            CreatedAt = DateTime.UtcNow
        };
        payment.SetBoletoPayment(
            faker.Random.AlphaNumeric(12),
            faker.Internet.Url(),
            faker.Random.AlphaNumeric(48));
        return payment;
    }
}
