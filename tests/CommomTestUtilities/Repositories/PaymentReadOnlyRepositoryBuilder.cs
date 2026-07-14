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

    public void BuildGetByOrderIdAsync(Payment? payment)
    {
        _mock.Setup(x => x.GetByOrderIdAsync(It.IsAny<Guid>())).ReturnsAsync(payment);
    }

    public void BuildGetByStripeIdAsync(Payment? payment)
    {
        _mock.Setup(x => x.GetByStripeIdAsync(It.IsAny<string>())).ReturnsAsync(payment);
    }

    public IPaymentReadOnlyRepository Build() => _mock.Object;
}
