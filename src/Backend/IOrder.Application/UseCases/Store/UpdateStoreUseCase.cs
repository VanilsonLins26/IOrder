using IOrder.Application.Services.LoggedUser;
using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Request;
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

public class UpdateStoreUseCase : IUpdateStoreUseCase
{
    private readonly IStoreReadOnlyRepository _readOnlyRepository;
    private readonly IStoreWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly ILoggedUserService _loggedUserService;
    private readonly IStorePermissionService _storePermissionService;

    public UpdateStoreUseCase(IStoreReadOnlyRepository readOnlyRepository, IStoreWriteOnlyRepository writeOnlyRepository, IUnitOfWork uof, ILoggedUserService loggedUserService, IStorePermissionService storePermissionService)
    {
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _loggedUserService = loggedUserService;
        _storePermissionService = storePermissionService;
    }
    public async Task<StoreResponseDto> Execute(UpdateStoreRequestDto request, Guid storeId)
    {
        var store = await _writeOnlyRepository.GetByIdTracking(storeId) ?? throw new NotFoundException([ResourceMessagesException.STORE_NOT_FOUND]);

        await _storePermissionService.ValidateStoreOwnerAsync(store);

        await Validate(request, store.Name!);

        request.Adapt(store);

        await _uof.Commit();

        return store.Adapt<StoreResponseDto>();
    }

    private async Task Validate(UpdateStoreRequestDto request, string storeName)
    {
        

        var validator = new UpdateStoreValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }

        var nameExists = await _readOnlyRepository.NameExists(request.Name!);

        if (nameExists && request.Name != storeName)
            throw new ErrorOnValidationException([ResourceMessagesException.NAME_ALREADY_EXISTS]);

        


    }
}
