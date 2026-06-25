using IOrder.Communication.Request;
using IOrder.Communication.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store;

public interface IUpdateOpeningHourUseCase
{
    Task<StoreResponseDto> Execute(UpdateOpeningHourRequestDto request, Guid storeId);
}
