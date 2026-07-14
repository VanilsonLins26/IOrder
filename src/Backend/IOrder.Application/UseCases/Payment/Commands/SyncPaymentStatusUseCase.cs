using IOrder.Application.Services.Payment;
using IOrder.Domain.Events;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Repositories.Payment;
using IOrder.Domain.Services;

namespace IOrder.Application.UseCases.Payment.Commands;

public interface ISyncPaymentStatusUseCase
{
    Task Execute(Guid orderId);
}

public class SyncPaymentStatusUseCase : ISyncPaymentStatusUseCase
{
    private readonly IPaymentReadOnlyRepository _paymentReadOnlyRepository;
    private readonly IPaymentWriteOnlyRepository _paymentWriteOnlyRepository;
    private readonly IOrderReadOnlyRepository _orderReadOnlyRepository;
    private readonly IOrderWriteOnlyRepository _orderWriteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly IPaymentService _paymentService;

    public SyncPaymentStatusUseCase(
        IPaymentReadOnlyRepository paymentReadOnlyRepository,
        IPaymentWriteOnlyRepository paymentWriteOnlyRepository,
        IOrderReadOnlyRepository orderReadOnlyRepository,
        IOrderWriteOnlyRepository orderWriteOnlyRepository,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher domainEventDispatcher,
        IPaymentService paymentService)
    {
        _paymentReadOnlyRepository = paymentReadOnlyRepository;
        _paymentWriteOnlyRepository = paymentWriteOnlyRepository;
        _orderReadOnlyRepository = orderReadOnlyRepository;
        _orderWriteOnlyRepository = orderWriteOnlyRepository;
        _unitOfWork = unitOfWork;
        _domainEventDispatcher = domainEventDispatcher;
        _paymentService = paymentService;
    }

    public async Task Execute(Guid orderId)
    {
        var payment = await _paymentReadOnlyRepository.GetByOrderIdAsync(orderId);
        if (payment == null || string.IsNullOrEmpty(payment.StripePaymentIntentId))
            return;

        if (payment.Status == Domain.Entities.Enums.PaymentStatus.Approved)
            return;

        var order = await _orderReadOnlyRepository.GetByIdAsync(orderId);
        if (order == null) return;

        var intentResponse = await _paymentService.GetPaymentByStripeIdAsync(payment.StripePaymentIntentId);
        if (intentResponse == null) return;

        bool updated = false;

        if (intentResponse.Status == Communication.Enums.PaymentStatusDto.Approved && payment.Status != Domain.Entities.Enums.PaymentStatus.Approved)
        {
            payment.Approve();
            order.MarkAsPaid();
            order.AddDomainEvent(new PaymentApprovedEvent(order.Id, payment.Id, payment.Amount));
            updated = true;
        }
        else if (intentResponse.Status == Communication.Enums.PaymentStatusDto.Rejected && payment.Status != Domain.Entities.Enums.PaymentStatus.Rejected)
        {
            payment.Reject();
            order.AddDomainEvent(new PaymentRejectedEvent(order.Id, payment.Id, payment.Amount, "Pagamento cancelado no Stripe"));
            updated = true;
        }

        if (updated)
        {
            _paymentWriteOnlyRepository.Update(payment);
            _orderWriteOnlyRepository.Update(order);
            await _unitOfWork.Commit();

            var events = order.DomainEvents.ToList();
            order.ClearDomainEvents();
            await _domainEventDispatcher.DispatchAsync(events);
        }
    }
}
