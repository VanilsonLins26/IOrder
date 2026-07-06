using IOrder.Communication.Request;
using IOrder.Communication.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Cart.Queries;

public interface IGetCartUseCase
{
    Task<CartResponseDto> Execute();
}
