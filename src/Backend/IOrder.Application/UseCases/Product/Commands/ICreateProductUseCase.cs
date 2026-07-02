using IOrder.Communication.Request;
using IOrder.Communication.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Product.Commands;

public interface ICreateProductUseCase
{

    Task<ProductResponseDto> Execute(ProductRequestDto productRequest);
}

