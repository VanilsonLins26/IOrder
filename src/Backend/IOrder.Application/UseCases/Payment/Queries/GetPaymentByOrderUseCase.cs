using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Payment;
using IOrder.Domain.Security.Services;
using Mapster;

namespace IOrder.Application.UseCases.Payment.Queries;

public class GetPaymentByOrderUseCase : IGetPaymentByOrderUseCase
{
    private readonly IPaymentReadOnlyRepository _paymentReadOnlyRepository;
    private readonly ILoggedUserService _loggedUserService;

    public GetPaymentByOrderUseCase(
        IPaymentReadOnlyRepository paymentReadOnlyRepository,
        ILoggedUserService loggedUserService)
    {
        _paymentReadOnlyRepository = paymentReadOnlyRepository;
        _loggedUserService = loggedUserService;
    }

    public async Task<PaymentResponseDto?> Execute(Guid orderId)
    {
        var payment = await _paymentReadOnlyRepository.GetByOrderIdAsync(orderId);
        return payment?.Adapt<PaymentResponseDto>();
    }
}
