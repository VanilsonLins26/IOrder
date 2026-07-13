using IOrder.Application.Services.Payment;
using IOrder.Communication.Response;
using Moq;

namespace CommomTestUtilities.Services;

public class PaymentServiceBuilder
{
    private readonly Mock<IPaymentService> _mock;

    public PaymentServiceBuilder()
    {
        _mock = new Mock<IPaymentService>();
    }

    public PaymentServiceBuilder CreatePixPaymentAsync(PaymentResponseDto response)
    {
        _mock.Setup(s => s.CreatePixPaymentAsync(
            It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync(response);
        return this;
    }

    public PaymentServiceBuilder CreateCardPaymentAsync(PaymentResponseDto response)
    {
        _mock.Setup(s => s.CreateCardPaymentAsync(
            It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<int>(),
            It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .ReturnsAsync(response);
        return this;
    }

    public PaymentServiceBuilder CreateBoletoPaymentAsync(PaymentResponseDto response)
    {
        _mock.Setup(s => s.CreateBoletoPaymentAsync(
            It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync(response);
        return this;
    }

    public PaymentServiceBuilder ProcessWebhookAsync(PaymentResponseDto? response)
    {
        _mock.Setup(s => s.ProcessWebhookAsync(
            It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync(response);
        return this;
    }

    public PaymentServiceBuilder GetPaymentByMercadoPagoIdAsync(PaymentResponseDto? response)
    {
        _mock.Setup(s => s.GetPaymentByMercadoPagoIdAsync(
            It.IsAny<string>()))
            .ReturnsAsync(response);
        return this;
    }

    public IPaymentService Build() => _mock.Object;
}
