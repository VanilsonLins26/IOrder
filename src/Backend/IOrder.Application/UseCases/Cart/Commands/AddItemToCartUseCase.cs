using FluentValidation;
using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Cart;
using IOrder.Domain.Repositories.Customization;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions.ExceptionBase;
using IOrder.Exceptions;
using IOrder.Domain.Repositories.Product;
using Mapster;

namespace IOrder.Application.UseCases.Cart.Commands;

public class AddItemToCartUseCase : IAddItemToCartUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly ICartReadOnlyRepository _readOnlyRepositoy;
    private readonly ICartWriteOnlyRepository _writeOnlyRepository;
    private readonly IValidator<AddItemToCartRequestDto> _validator;
    private readonly IProductReadOnlyRepository _productReadOnlyRepository;
    private readonly ICustomizationReadOnlyRepository _customizationRepository;

    public AddItemToCartUseCase(
        ILoggedUserService loggedUserService, 
        ICartReadOnlyRepository readOnlyRepositoy, 
        ICartWriteOnlyRepository writeOnlyRepository, 
        IValidator<AddItemToCartRequestDto> validator,
        IProductReadOnlyRepository productReadOnlyRepository,
        ICustomizationReadOnlyRepository customizationRepository)
    {
        _loggedUserService = loggedUserService;
        _readOnlyRepositoy = readOnlyRepositoy;
        _writeOnlyRepository = writeOnlyRepository;
        _validator = validator;
        _productReadOnlyRepository = productReadOnlyRepository;
        _customizationRepository = customizationRepository;
    }

    public async Task<CartResponseDto> Execute(AddItemToCartRequestDto request)
    {
        var userId = _loggedUserService.GetUserId();

        await Validate(request);

        var product = await _productReadOnlyRepository.GetByIdAsync(request.ProductId) ?? throw new NotFoundException([ResourceMessagesException.PRODUCT_NOT_FOUND]);
        var cart = await _readOnlyRepositoy.GetCartAsync(userId) ?? new Domain.Entities.Cart { UserId = userId };

        if (cart.Items.Count != 0)
        {
            var existingStoreId = cart.Items
                .Select(i => i.StoreId)
                .FirstOrDefault(sid => sid != Guid.Empty);

            if (existingStoreId == Guid.Empty)
            {
                var existingProduct = await _productReadOnlyRepository.GetByIdAsync(cart.Items.First().ProductId);
                existingStoreId = existingProduct?.StoreId ?? Guid.Empty;
            }

            if (existingStoreId != Guid.Empty && existingStoreId != product.StoreId)
            {
                throw new ErrorOnValidationException([ResourceMessagesException.CART_DIFFERENT_STORE]);
            }
        }

        var cartItem = request.Adapt<CartItem>();
        cartItem.UnitPrice = product.Price;
        cartItem.ProductName = product.Name;
        cartItem.ProductImageUrl = product.ImageUrl;
        cartItem.StoreId = product.StoreId;

        if (request.SelectedOptionIds.Count > 0)
        {
            var groups = await _customizationRepository.GetByProductId(request.ProductId);
            var allOptions = groups.SelectMany(g => g.Options).ToList();
            var selected = allOptions.Where(o => request.SelectedOptionIds.Contains(o.Id)).ToList();

            var priceModifier = selected.Sum(o => o.PriceModifier);
            cartItem.UnitPrice += priceModifier;

            cartItem.SelectedOptions = selected.Select(o => new SelectedOption
            {
                OptionId = o.Id,
                OptionName = o.Name,
                GroupName = groups.First(g => g.Options.Any(go => go.Id == o.Id)).Name,
                PriceModifier = o.PriceModifier
            }).ToList();
        }

        cart.UserEmail ??= _loggedUserService.GetUserEmail();
        cart.UserPhone ??= _loggedUserService.GetUserPhone();
        cart.AddCartItem(cartItem);

        await _writeOnlyRepository.SaveCartAsync(cart);

        return cart.Adapt<CartResponseDto>();
    }

    private async Task Validate(AddItemToCartRequestDto request) 
    {
        var result = await _validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
