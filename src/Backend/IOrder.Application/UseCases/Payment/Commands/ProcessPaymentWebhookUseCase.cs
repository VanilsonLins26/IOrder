using IOrder.Application.Services.Payment;
using IOrder.Communication.Response;
using IOrder.Domain.Events;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Repositories.Payment;
using IOrder.Domain.Services;

namespace IOrder.Application.UseCases.Payment.Commands;

public class ProcessPaymentWebhookUseCase : IProcessPaymentWebhookUseCase
{
    private readonly IPaymentService _paymentService;
    private readonly IPaymentWriteOnlyRepository _paymentWriteOnlyRepository;
    private readonly IOrderWriteOnlyRepository _orderWriteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public ProcessPaymentWebhookUseCase(
        IPaymentService paymentService,
        IPaymentWriteOnlyRepository paymentWriteOnlyRepository,
        IOrderWriteOnlyRepository orderWriteOnlyRepository,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _paymentService = paymentService;
        _paymentWriteOnlyRepository = paymentWriteOnlyRepository;
        _orderWriteOnlyRepository = orderWriteOnlyRepository;
        _unitOfWork = unitOfWork;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<PaymentResponseDto?> Execute(string payload, string? signature)
    {
        var response = await _paymentService.ProcessWebhookAsync(payload, signature);
        if (response is null) return null;

        var payment = await _paymentWriteOnlyRepository.GetByMercadoPagoId(response.Id.ToString());
        if (payment is null) return response;

        var order = await _orderWriteOnlyRepository.GetByIdTracking(payment.OrderId);
        if (order is null) return response;

        if (response.Status == Communication.Enums.PaymentStatusDto.Approved)
        {
            payment.Approve();
            order.MarkAsPaid();
            order.AddDomainEvent(new PaymentApprovedEvent(order.Id, payment.Id, payment.Amount));
        }
        else if (response.Status == Communication.Enums.PaymentStatusDto.Rejected)
        {
            payment.Reject();
            order.AddDomainEvent(new PaymentRejectedEvent(order.Id, payment.Id, payment.Amount, "Pagamento rejeitado"));
        }

        _paymentWriteOnlyRepository.Update(payment);
        _orderWriteOnlyRepository.Update(order);
        await _unitOfWork.Commit();

        var events = order.DomainEvents.ToList();
        order.ClearDomainEvents();
        await _domainEventDispatcher.DispatchAsync(events);

        return response;
    }
}
