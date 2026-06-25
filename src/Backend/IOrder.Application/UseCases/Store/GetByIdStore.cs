using IOrder.Application.Services.LoggedUser;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Store;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store;

public class GetByIdStore : IGetByIdStoreUseCase
{
    private readonly IStoreReadOnlyRepository _readOnlyRepository;

    public GetByIdStore(IStoreReadOnlyRepository readOnlyRepository)
    {
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<StoreResponseDto> Execute(Guid Id)
    {
        var store = await _readOnlyRepository.GetByIdAsync(Id) ?? throw new NotFoundException([ResourceMessagesException.STORE_NOT_FOUND]);

        return store.Adapt<StoreResponseDto>();
    }
}
