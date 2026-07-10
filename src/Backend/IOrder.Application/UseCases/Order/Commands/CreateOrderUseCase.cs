using FluentValidation;
using IOrder.Communication.Response;
using IOrder.Domain.Events;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Cart;
using IOrder.Domain.Repositories.Coupon;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Repositories.Product;
using IOrder.Domain.Repositories.Profile;
using IOrder.Domain.Security.Services;
using IOrder.Domain.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Order.Commands;

public class CreateOrderUseCase : ICreateOrderUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly ICartReadOnlyRepository _cartReadOnlyRepository;
    private readonly ICartWriteOnlyRepository _cartWriteOnlyRepository;
    private readonly IOrderWriteOnlyRepository _orderWriteOnlyRepository;
    private readonly IProductReadOnlyRepository _productReadOnlyRepository;
    private readonly ICouponReadOnlyRepository _couponReadOnlyRepository;
    private readonly IProfileReadOnlyRepository _profileReadOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly IValidator<Communication.Request.CreateOrderRequestDto> _validator;

    public CreateOrderUseCase(
        ILoggedUserService loggedUserService,
        ICartReadOnlyRepository cartReadOnlyRepository,
        ICartWriteOnlyRepository cartWriteOnlyRepository,
        IOrderWriteOnlyRepository orderWriteOnlyRepository,
        IProductReadOnlyRepository productReadOnlyRepository,
        ICouponReadOnlyRepository couponReadOnlyRepository,
        IProfileReadOnlyRepository profileReadOnlyRepository,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher domainEventDispatcher,
        IValidator<Communication.Request.CreateOrderRequestDto> validator)
    {
        _loggedUserService = loggedUserService;
        _cartReadOnlyRepository = cartReadOnlyRepository;
        _cartWriteOnlyRepository = cartWriteOnlyRepository;
        _orderWriteOnlyRepository = orderWriteOnlyRepository;
        _productReadOnlyRepository = productReadOnlyRepository;
        _couponReadOnlyRepository = couponReadOnlyRepository;
        _profileReadOnlyRepository = profileReadOnlyRepository;
        _unitOfWork = unitOfWork;
        _domainEventDispatcher = domainEventDispatcher;
        _validator = validator;
    }

    public async Task<OrderResponseDto> Execute(Communication.Request.CreateOrderRequestDto request)
    {
        await Validate(request);

        var userId = _loggedUserService.GetUserId();

        var cart = await _cartReadOnlyRepository.GetCartAsync(userId)
            ?? throw new ErrorOnValidationException([ResourceMessagesException.INVALID_CART]);

        if (cart.Items.Count == 0)
            throw new ErrorOnValidationException([ResourceMessagesException.ORDER_EMPTY_CART]);

        var firstItem = cart.Items.First();
        var storeId = firstItem.StoreId;

        if (storeId == Guid.Empty)
        {
            var product = await _productReadOnlyRepository.GetByIdAsync(firstItem.ProductId);
            storeId = product?.StoreId ?? Guid.Empty;
        }

        decimal discountedTotal = cart.CartTotal;
        decimal? discountValue = null;

        if (!string.IsNullOrEmpty(cart.CouponCode))
        {
            var coupon = await _couponReadOnlyRepository.GetByCodeAsync(cart.CouponCode);
            if (coupon is not null && coupon.IsValid() && (!coupon.MinPurchaseAmount.HasValue || cart.CartTotal >= coupon.MinPurchaseAmount.Value))
            {
                discountValue = coupon.CalculateDiscount(cart.CartTotal);
                discountedTotal = cart.CartTotal - discountValue.Value;
            }
        }

        var customerEmail = _loggedUserService.GetUserEmail();

        var profile = await _profileReadOnlyRepository.GetByUserId(userId);
        var phone = request.CustomerPhone ?? profile?.Phone;

        var order = new Domain.Entities.Order
        {
            UserId = userId,
            CustomerEmail = customerEmail,
            CustomerPhone = phone,
            StoreId = storeId,
            TotalAmount = discountedTotal,
            OriginalAmount = cart.CartTotal,
            CouponCode = cart.CouponCode,
            DiscountValue = discountValue,
            DiscountedTotal = discountedTotal,
            CustomerNotes = request.CustomerNotes,
            DeliveryDate = request.DeliveryDate
        };

        foreach (var cartItem in cart.Items)
        {
            var orderItem = new Domain.Entities.OrderItem
            {
                ProductId = cartItem.ProductId,
                ProductName = cartItem.ProductName,
                ProductImageUrl = cartItem.ProductImageUrl,
                UnitPrice = cartItem.UnitPrice,
                Quantity = cartItem.Quantity,
                Customize = cartItem.Customize
            };

            orderItem.UpdateImageUrls(cartItem.ImageUrls);

            order.AddItem(orderItem);
        }

        order.AddDomainEvent(new OrderCreatedEvent(order.Id, userId, storeId, order.TotalAmount));

        await _orderWriteOnlyRepository.Create(order);
        await _unitOfWork.Commit();

        var events = order.DomainEvents.ToList();
        order.ClearDomainEvents();
        await _domainEventDispatcher.DispatchAsync(events);

        await _cartWriteOnlyRepository.DeleteCartAsync(userId);

        return order.Adapt<OrderResponseDto>();
    }

    private async Task Validate(Communication.Request.CreateOrderRequestDto request)
    {
        var result = await _validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
