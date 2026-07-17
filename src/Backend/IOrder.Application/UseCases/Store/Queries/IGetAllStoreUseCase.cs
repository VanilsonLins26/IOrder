using IOrder.Communication.Request;
using IOrder.Communication.Response;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Store.Queries;

public interface IGetAllStoreUseCase
{
    Task<PagedResponse<StoreResponseDto>> Execute(StoreSearchRequestDto filter);
}

