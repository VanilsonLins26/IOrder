using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Cart;
using IOrder.Domain.Repositories.Coupon;
using IOrder.Domain.Repositories.Product;
using IOrder.Domain.Security.Services;
using Mapster;

namespace IOrder.Application.UseCases.Cart.Queries;

public class GetCartUseCase : IGetCartUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly ICartReadOnlyRepository _readOnlyRepositoy;
    private readonly IProductReadOnlyRepository _productReadOnlyRepository;
    private readonly ICartWriteOnlyRepository _writeOnlyRepository;
    private readonly ICouponReadOnlyRepository _couponReadOnlyRepository;

    public GetCartUseCase(
        ILoggedUserService loggedUserService, 
        ICartReadOnlyRepository readOnlyRepositoy, 
        IProductReadOnlyRepository productReadOnlyRepository,
        ICartWriteOnlyRepository writeOnlyRepository,
        ICouponReadOnlyRepository couponReadOnlyRepository)
    {
        _loggedUserService = loggedUserService;
        _readOnlyRepositoy = readOnlyRepositoy;
        _productReadOnlyRepository = productReadOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _couponReadOnlyRepository = couponReadOnlyRepository;
    }

    public async Task<CartResponseDto> Execute()
    {
        var userId = _loggedUserService.GetUserId();

        var cart = await _readOnlyRepositoy.GetCartAsync(userId) ?? new Domain.Entities.Cart { UserId = userId };

        if (!cart.Items.Any())
        {
            var emptyResponse = cart.Adapt<CartResponseDto>();
            return await ApplyDiscount(emptyResponse);
        }

        var productIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();
        var prices = await _productReadOnlyRepository.GetProductPricesByIds(productIds);

        bool cartModified = false;

        foreach (var item in cart.Items.ToList()) 
        {
            if (prices.TryGetValue(item.ProductId, out var productPrice))
            {
                if (item.UnitPrice != productPrice)
                {
                    cart.UpdateItemPrice(productPrice, item.Id);
                    cartModified = true;
                }
            }
            else
            {
                cart.RemoveCartItem(item.Id);
                cartModified = true;
            }
        }

        if (cartModified)
        {
            await _writeOnlyRepository.SaveCartAsync(cart);
        }

        var response = cart.Adapt<CartResponseDto>();
        return await ApplyDiscount(response);
    }

    private async Task<CartResponseDto> ApplyDiscount(CartResponseDto response)
    {
        if (string.IsNullOrEmpty(response.CouponCode))
            return response;

        var coupon = await _couponReadOnlyRepository.GetByCodeAsync(response.CouponCode);
        if (coupon is null || !coupon.IsValid())
            return response;

        if (coupon.MinPurchaseAmount.HasValue && response.CartTotal < coupon.MinPurchaseAmount.Value)
            return response;

        response.DiscountValue = coupon.CalculateDiscount(response.CartTotal);
        response.DiscountedTotal = response.CartTotal - response.DiscountValue;
        return response;
    }
}
