using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Payment;
using Moq;

namespace CommomTestUtilities.Repositories;

public class PaymentWriteOnlyRepositoryBuilder
{
    private readonly Mock<IPaymentWriteOnlyRepository> _mock;

    public PaymentWriteOnlyRepositoryBuilder()
    {
        _mock = new Mock<IPaymentWriteOnlyRepository>();
    }

    public void BuildGetByIdTracking(Payment payment)
    {
        _mock.Setup(x => x.GetByIdTracking(payment.Id)).ReturnsAsync(payment);
    }

    public IPaymentWriteOnlyRepository Build() => _mock.Object;
}
