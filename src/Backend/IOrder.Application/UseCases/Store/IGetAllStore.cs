using IOrder.Communication.Response;
using IOrder.Domain.Pagination;
using IOrder.Domain.SeedWork.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store;

public interface IGetAllStore
{
    Task<PagedList<StoreResponseDto>> Execute(StoreSearchQuery filter);
}
