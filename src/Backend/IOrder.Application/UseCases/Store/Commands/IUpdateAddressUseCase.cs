using IOrder.Communication.Request;
using IOrder.Communication.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store.Commands;

public interface IUpdateAddressUseCase
{
    Task<StoreResponseDto> Execute(AddressRequestDto request, Guid StoreId);
}

