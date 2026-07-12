using IOrder.Application.Services.Payment;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Repositories.Payment;
using IOrder.Domain.Security.Services;
using IOrder.Domain.Services;
using IOrder.Exceptions;

namespace IOrder.Application.UseCases.Payment.Commands;

public class CreatePaymentUseCase : ICreatePaymentUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly IOrderReadOnlyRepository _orderReadOnlyRepository;
    private readonly IPaymentWriteOnlyRepository _paymentWriteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly IPaymentService _paymentService;

    public CreatePaymentUseCase(
        ILoggedUserService loggedUserService,
        IOrderReadOnlyRepository orderReadOnlyRepository,
        IPaymentWriteOnlyRepository paymentWriteOnlyRepository,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher domainEventDispatcher,
        IPaymentService paymentService)
    {
        _loggedUserService = loggedUserService;
        _orderReadOnlyRepository = orderReadOnlyRepository;
        _paymentWriteOnlyRepository = paymentWriteOnlyRepository;
        _unitOfWork = unitOfWork;
        _domainEventDispatcher = domainEventDispatcher;
        _paymentService = paymentService;
    }

    public async Task<PaymentResponseDto> Execute(CreatePaymentRequestDto request)
    {
        var userId = _loggedUserService.GetUserId();

        var order = await _orderReadOnlyRepository.GetByIdAsync(request.OrderId)
            ?? throw new NotFoundException([ResourceMessagesException.ORDER_NOT_FOUND]);

        if (order.UserId != userId)
            throw new UnauthorizedStoreException([ResourceMessagesException.ORDER_NOT_FOUND]);

        if (order.Status != OrderStatus.AwaitingPayment)
            throw new ErrorOnValidationException(["Pedido não está aguardando pagamento."]);

        var paymentResponse = request.Method switch
        {
            Communication.Enums.PaymentMethodDto.Pix => await _paymentService.CreatePixPaymentAsync(
                order.Id, order.TotalAmount, request.PayerEmail, request.PayerIdentificationNumber),

            Communication.Enums.PaymentMethodDto.CreditCard => await _paymentService.CreateCardPaymentAsync(
                order.Id, order.TotalAmount, request.CardToken ?? "", request.Installments ?? 1, request.PayerEmail, request.PayerIdentificationNumber),

            Communication.Enums.PaymentMethodDto.Boleto => await _paymentService.CreateBoletoPaymentAsync(
                order.Id, order.TotalAmount, request.PayerEmail, request.PayerIdentificationNumber),

            _ => throw new ErrorOnValidationException(["Método de pagamento inválido."])
        };

        return paymentResponse;
    }
}
