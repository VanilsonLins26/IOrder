using IOrder.Domain.Security.Services;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Store;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store.Queries;

public class GetByIdStoreUseCase : IGetByIdStoreUseCase
{
    private readonly IStoreReadOnlyRepository _readOnlyRepository;

    public GetByIdStoreUseCase(IStoreReadOnlyRepository readOnlyRepository)
    {
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<StoreResponseDto> Execute(Guid Id, double? userLatitude = null, double? userLongitude = null)
    {
        var store = await _readOnlyRepository.GetByIdWithDistanceAsync(Id, userLatitude, userLongitude)
            ?? throw new NotFoundException([ResourceMessagesException.STORE_NOT_FOUND]);

        return store.Adapt<StoreResponseDto>();
    }
}

