using IOrder.Application.Services.Payment;
using IOrder.Domain.Repositories.Payment;

namespace IOrder.Application.UseCases.Payment.Commands;

public interface IUpdatePaymentIntentSaveCardUseCase
{
    Task Execute(Guid orderId, bool saveCard);
}

public class UpdatePaymentIntentSaveCardUseCase : IUpdatePaymentIntentSaveCardUseCase
{
    private readonly IPaymentReadOnlyRepository _paymentRepository;
    private readonly IPaymentService _paymentService;

    public UpdatePaymentIntentSaveCardUseCase(IPaymentReadOnlyRepository paymentRepository, IPaymentService paymentService)
    {
        _paymentRepository = paymentRepository;
        _paymentService = paymentService;
    }

    public async Task Execute(Guid orderId, bool saveCard)
    {
        var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
        if (payment == null || string.IsNullOrEmpty(payment.StripePaymentIntentId))
            return;

        await _paymentService.UpdatePaymentIntentSetupFutureUsageAsync(payment.StripePaymentIntentId, saveCard);
    }
}
