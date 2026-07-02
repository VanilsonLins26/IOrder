using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Store;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store.Queries;

public class GetAllStoreUseCase : IGetAllStoreUseCase
{
    private readonly IStoreReadOnlyRepository _readOnlyRepository;

    public GetAllStoreUseCase(IStoreReadOnlyRepository readOnlyRepository)
    {
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<PagedResponse<StoreResponseDto>> Execute(StoreSearchRequestDto request)
    {
        var criteria = new StoreSearchCriteria(
            request.PageNumber, 
            request.PageSize, 
            request.Name, 
            request.CategoryId, 
            request.OrderBy, 
            request.IsDescending);

        var storesTuple = await _readOnlyRepository.GetAllPaged(criteria);

        return new PagedResponse<StoreResponseDto>(
            storesTuple.Items.Adapt<List<StoreResponseDto>>(), 
            storesTuple.TotalCount, 
            request.PageNumber, 
            request.PageSize);
    }
}

