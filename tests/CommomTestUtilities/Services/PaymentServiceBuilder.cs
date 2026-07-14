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

    public void BuildCreatePaymentIntent(PaymentIntentResponseDto response)
    {
        _mock.Setup(m => m.CreatePaymentIntentAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string>())).ReturnsAsync(response);
    }

    public void BuildProcessWebhook(PaymentResponseDto? response)
    {
        _mock.Setup(m => m.ProcessWebhookAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(response);
    }

    public void BuildGetPaymentByStripeId(PaymentResponseDto? response)
    {
        _mock.Setup(m => m.GetPaymentByStripeIdAsync(It.IsAny<string>())).ReturnsAsync(response);
    }

    public IPaymentService Build() => _mock.Object;
}
