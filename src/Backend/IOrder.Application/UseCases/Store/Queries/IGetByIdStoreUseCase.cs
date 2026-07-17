using IOrder.Communication.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store.Queries;

public interface IGetByIdStoreUseCase
{
    Task<StoreResponseDto> Execute(Guid Id, double? userLatitude = null, double? userLongitude = null);
}

