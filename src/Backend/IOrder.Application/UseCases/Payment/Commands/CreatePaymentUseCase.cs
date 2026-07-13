using FluentValidation;
using IOrder.Application.Services.Payment;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Repositories.Payment;
using IOrder.Domain.Repositories.Profile;
using IOrder.Domain.Security.Services;
using IOrder.Domain.Events;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Payment.Commands;

public class CreatePaymentUseCase : ICreatePaymentUseCase
{
    private readonly IValidator<CreatePaymentRequestDto> _validator;
    private readonly ILoggedUserService _loggedUserService;
    private readonly IOrderReadOnlyRepository _orderReadOnlyRepository;
    private readonly IOrderWriteOnlyRepository _orderWriteOnlyRepository;
    private readonly IPaymentReadOnlyRepository _paymentReadOnlyRepository;
    private readonly IProfileReadOnlyRepository _profileReadOnlyRepository;
    private readonly IUserCardReadOnlyRepository _userCardReadOnlyRepository;
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentUseCase(
        IValidator<CreatePaymentRequestDto> validator,
        ILoggedUserService loggedUserService,
        IOrderReadOnlyRepository orderReadOnlyRepository,
        IOrderWriteOnlyRepository orderWriteOnlyRepository,
        IPaymentReadOnlyRepository paymentReadOnlyRepository,
        IProfileReadOnlyRepository profileReadOnlyRepository,
        IUserCardReadOnlyRepository userCardReadOnlyRepository,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _loggedUserService = loggedUserService;
        _orderReadOnlyRepository = orderReadOnlyRepository;
        _orderWriteOnlyRepository = orderWriteOnlyRepository;
        _paymentReadOnlyRepository = paymentReadOnlyRepository;
        _profileReadOnlyRepository = profileReadOnlyRepository;
        _userCardReadOnlyRepository = userCardReadOnlyRepository;
        _paymentService = paymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentResponseDto> Execute(CreatePaymentRequestDto request)
    {
        await Validate(request);

        var userId = _loggedUserService.GetUserId();

        var order = await _orderReadOnlyRepository.GetByIdAsync(request.OrderId)
            ?? throw new NotFoundException([ResourceMessagesException.ORDER_NOT_FOUND]);

        if (order.UserId != userId)
            throw new UnauthorizedStoreException([ResourceMessagesException.ORDER_NOT_FOUND]);

        if (order.Status != OrderStatus.AwaitingPayment)
            throw new ErrorOnValidationException([ResourceMessagesException.PAYMENT_ORDER_NOT_AWAITING]);

        var existingPayment = await _paymentReadOnlyRepository.GetByOrderIdAsync(order.Id);
        if (existingPayment is not null && existingPayment.Status == Domain.Entities.Enums.PaymentStatus.Pending)
            return existingPayment.Adapt<PaymentResponseDto>();

        var paymentResponse = request.Method switch
        {
            Communication.Enums.PaymentMethodDto.Pix => await _paymentService.CreatePixPaymentAsync(
                order.Id, order.TotalAmount, request.PayerEmail, request.PayerIdentificationNumber),

            Communication.Enums.PaymentMethodDto.CreditCard => 
                await HandleCardPaymentAsync(order.Id, order.TotalAmount, request, userId),

            Communication.Enums.PaymentMethodDto.Boleto => await _paymentService.CreateBoletoPaymentAsync(
                order.Id, order.TotalAmount, request.PayerEmail, request.PayerIdentificationNumber),

            _ => throw new ErrorOnValidationException([ResourceMessagesException.PAYMENT_METHOD_INVALID])
        };

        if (paymentResponse.Status == Communication.Enums.PaymentStatusDto.Approved)
        {
            var trackedOrder = await _orderWriteOnlyRepository.GetByIdTracking(order.Id);
            if (trackedOrder != null)
            {
                trackedOrder.MarkAsPaid();
                trackedOrder.AddDomainEvent(new PaymentApprovedEvent(trackedOrder.Id, paymentResponse.Id, paymentResponse.Amount));
                _orderWriteOnlyRepository.Update(trackedOrder);
                await _unitOfWork.Commit();
            }
        }

        return paymentResponse;
    }

    private async Task<PaymentResponseDto> HandleCardPaymentAsync(Guid orderId, decimal amount, CreatePaymentRequestDto request, string userId)
    {
        return await _paymentService.CreateCardPaymentAsync(
            orderId, amount, request.CardToken ?? "", request.Installments ?? 1, request.PayerEmail, request.PayerIdentificationNumber, null);
    }

    private async Task Validate(CreatePaymentRequestDto request)
    {
        var result = await _validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
