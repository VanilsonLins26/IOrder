using IOrder.Application.Services.LoggedUser;
using IOrder.Application.Services.StorePermission;
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

public class DeleteStoreUseCase : IDeleteStoreUseCase
{
    private readonly IStoreWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly ILoggedUserService _loggedUserService;
    private readonly IStorePermissionService _storePermissionService;

    public DeleteStoreUseCase(IStoreWriteOnlyRepository writeOnlyRepository, IUnitOfWork uof, ILoggedUserService loggedUserService, IStorePermissionService storePermissionService)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _loggedUserService = loggedUserService;
        _storePermissionService = storePermissionService;
    }

    public async Task<StoreResponseDto> Execute(Guid id)
    {
        var store = await _writeOnlyRepository.GetByIdTracking(id) ?? throw new NotFoundException([ResourceMessagesException.STORE_NOT_FOUND]);

        await _storePermissionService.ValidateStoreOwnerAsync(store);

        _writeOnlyRepository.Delete(store);

        await _uof.Commit();

        return store.Adapt<StoreResponseDto>();
    }
}
