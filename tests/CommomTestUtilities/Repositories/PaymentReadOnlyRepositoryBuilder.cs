using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Payment;
using Moq;

namespace CommomTestUtilities.Repositories;

public class PaymentReadOnlyRepositoryBuilder
{
    private readonly Mock<IPaymentReadOnlyRepository> _mock;

    public PaymentReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<IPaymentReadOnlyRepository>();
    }

    public PaymentReadOnlyRepositoryBuilder GetByOrderIdAsync(Payment? payment)
    {
        _mock.Setup(r => r.GetByOrderIdAsync(It.IsAny<Guid>())).ReturnsAsync(payment);
        return this;
    }

    public PaymentReadOnlyRepositoryBuilder GetByMercadoPagoIdAsync(Payment? payment)
    {
        _mock.Setup(r => r.GetByMercadoPagoIdAsync(It.IsAny<string>())).ReturnsAsync(payment);
        return this;
    }

    public IPaymentReadOnlyRepository Build() => _mock.Object;
}
