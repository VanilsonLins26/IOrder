using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Cart;
using IOrder.Domain.Repositories.Product;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace IOrder.Application.UseCases.Cart.Commands;

public class ChangeQuantityUseCase : IChangeQuantityUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly ICartReadOnlyRepository _readOnlyRepositoy;
    private readonly ICartWriteOnlyRepository _writeOnlyRepository;
    private readonly IValidator<ChangeCartItemQuantityRequestDto> _validator;

    public ChangeQuantityUseCase(ILoggedUserService loggedUserService, ICartReadOnlyRepository readOnlyRepositoy, ICartWriteOnlyRepository writeOnlyRepository, IValidator<ChangeCartItemQuantityRequestDto> validator)
    {
        _loggedUserService = loggedUserService;
        _readOnlyRepositoy = readOnlyRepositoy;
        _writeOnlyRepository = writeOnlyRepository;
        _validator = validator;
    }

    public async Task<CartResponseDto> Execute(ChangeCartItemQuantityRequestDto request)
    {

        var userId = _loggedUserService.GetUserId();

        await Validate(request);

        var cart = await _readOnlyRepositoy.GetCartAsync(userId) ?? throw new NotFoundException([ResourceMessagesException.INVALID_CART]);

        var result = cart.ChangeCartItemQuantity(request.NewQuantity, request.CartItemId);
        if (!result)
            throw new NotFoundException([ResourceMessagesException.CART_ITEM_NOT_FOUND]);

        cart.UserEmail ??= _loggedUserService.GetUserEmail();

        await _writeOnlyRepository.SaveCartAsync(cart);

        return cart.Adapt<CartResponseDto>();
    }

    private async Task Validate(ChangeCartItemQuantityRequestDto request)
    {
        var result = await _validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
