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

    public PaymentWriteOnlyRepositoryBuilder CreateAsync()
    {
        _mock.Setup(r => r.CreateAsync(It.IsAny<Payment>()))
            .Returns(Task.CompletedTask);
        return this;
    }

    public PaymentWriteOnlyRepositoryBuilder GetById(Payment? payment)
    {
        _mock.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync(payment);
        return this;
    }

    public PaymentWriteOnlyRepositoryBuilder GetByMercadoPagoId(Payment? payment)
    {
        _mock.Setup(r => r.GetByMercadoPagoId(It.IsAny<string>())).ReturnsAsync(payment);
        return this;
    }

    public PaymentWriteOnlyRepositoryBuilder Update()
    {
        _mock.Setup(r => r.Update(It.IsAny<Payment>())).Returns((Payment p) => p);
        return this;
    }

    public IPaymentWriteOnlyRepository Build() => _mock.Object;
}
