using IOrder.Communication.Response;
using IOrder.Domain.Pagination;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.SeedWork.Pagination;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store;

public class GetAllStore : IGetAllStore
{
    private readonly IStoreReadOnlyRepository _readOnlyRepository;

    public GetAllStore(IStoreReadOnlyRepository readOnlyRepository)
    {
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<PagedList<StoreResponseDto>> Execute(StoreSearchQuery filter)
    {
        var stores = await _readOnlyRepository.GetAllPaged(filter);

        return stores.Adapt<PagedList<StoreResponseDto>>();
    }
}
