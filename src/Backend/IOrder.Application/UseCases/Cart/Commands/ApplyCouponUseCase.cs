using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Cart;
using IOrder.Domain.Repositories.Coupon;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Cart.Commands;

public class ApplyCouponUseCase : IApplyCouponUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly ICartReadOnlyRepository _readOnlyRepositoy;
    private readonly ICartWriteOnlyRepository _writeOnlyRepository;
    private readonly ICouponReadOnlyRepository _couponReadOnlyRepository;
    private readonly FluentValidation.IValidator<ApplyCouponRequestDto> _validator;

    public ApplyCouponUseCase(
        ILoggedUserService loggedUserService, 
        ICartReadOnlyRepository readOnlyRepositoy, 
        ICartWriteOnlyRepository writeOnlyRepository,
        ICouponReadOnlyRepository couponReadOnlyRepository,
        FluentValidation.IValidator<ApplyCouponRequestDto> validator)
    {
        _loggedUserService = loggedUserService;
        _readOnlyRepositoy = readOnlyRepositoy;
        _writeOnlyRepository = writeOnlyRepository;
        _couponReadOnlyRepository = couponReadOnlyRepository;
        _validator = validator;
    }

    public async Task<CartResponseDto> Execute(ApplyCouponRequestDto request)
    {
        await Validate(request);

        var userId = _loggedUserService.GetUserId();

        var cart = await _readOnlyRepositoy.GetCartAsync(userId) ?? throw new NotFoundException([ResourceMessagesException.INVALID_CART]);

        await ValidateCoupon(request.CouponCode, cart.CartTotal);

        cart.CouponCode = request.CouponCode;

        await _writeOnlyRepository.SaveCartAsync(cart);

        var response = cart.Adapt<CartResponseDto>();

        var coupon = await _couponReadOnlyRepository.GetByCodeAsync(request.CouponCode);
        if (coupon is not null && coupon.IsValid() && (!coupon.MinPurchaseAmount.HasValue || cart.CartTotal >= coupon.MinPurchaseAmount.Value))
        {
            response.DiscountValue = coupon.CalculateDiscount(cart.CartTotal);
            response.DiscountedTotal = cart.CartTotal - response.DiscountValue;
        }

        return response;
    }

    private async Task Validate(ApplyCouponRequestDto request)
    {
        var result = await _validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }

    private async Task ValidateCoupon(string couponCode, decimal cartTotal)
    {
        var coupon = await _couponReadOnlyRepository.GetByCodeAsync(couponCode);

        if (coupon is null)
            throw new NotFoundException([ResourceMessagesException.COUPON_NOT_FOUND]);

        if (!coupon.Active || coupon.IsExpired())
            throw new ErrorOnValidationException([ResourceMessagesException.COUPON_EXPIRED]);

        if (coupon.IsUsageLimitReached())
            throw new ErrorOnValidationException([ResourceMessagesException.COUPON_USAGE_LIMIT]);

        if (coupon.MinPurchaseAmount.HasValue && cartTotal < coupon.MinPurchaseAmount.Value)
            throw new ErrorOnValidationException([ResourceMessagesException.COUPON_MIN_PURCHASE]);
    }
}
