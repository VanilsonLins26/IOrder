using IOrder.Communication.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store;

public interface IDeleteStoreUseCase
{
    Task<StoreResponseDto> Execute(Guid id);
}
