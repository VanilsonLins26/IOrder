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
using System.Text;

namespace IOrder.Application.UseCases.Cart.Queries;

public class GetCartUseCase : IGetCartUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly ICartReadOnlyRepository _readOnlyRepositoy;
    private readonly IProductReadOnlyRepository _productReadOnlyRepository;
    private readonly ICartWriteOnlyRepository _writeOnlyRepository;

    public GetCartUseCase(
        ILoggedUserService loggedUserService, 
        ICartReadOnlyRepository readOnlyRepositoy, 
        IProductReadOnlyRepository productReadOnlyRepository,
        ICartWriteOnlyRepository writeOnlyRepository)
    {
        _loggedUserService = loggedUserService;
        _readOnlyRepositoy = readOnlyRepositoy;
        _productReadOnlyRepository = productReadOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
    }

    public async Task<CartResponseDto> Execute()
    {
        var userId = _loggedUserService.GetUserId();

        var cart = await _readOnlyRepositoy.GetCartAsync(userId) ?? new Domain.Entities.Cart { UserId = userId };

        if (!cart.Items.Any())
        {
            return cart.Adapt<CartResponseDto>();
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

        return cart.Adapt<CartResponseDto>();
    }
}
