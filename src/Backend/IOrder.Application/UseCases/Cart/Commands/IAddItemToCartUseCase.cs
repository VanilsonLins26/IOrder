using IOrder.Communication.Request;
using IOrder.Communication.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Cart.Commands;

public interface IAddItemToCartUseCase
{
    Task<CartResponseDto> Execute(AddItemToCartRequestDto request);
}
