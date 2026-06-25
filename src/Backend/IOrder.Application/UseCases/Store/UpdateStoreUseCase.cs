using IOrder.Application.Services.LoggedUser;
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

    public UpdateStoreUseCase(IStoreReadOnlyRepository readOnlyRepository, IStoreWriteOnlyRepository writeOnlyRepository, IUnitOfWork uof, ILoggedUserService loggedUserService)
    {
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _loggedUserService = loggedUserService;
    }
    public async Task<StoreResponseDto> Execute(UpdateStoreRequestDto request, Guid storeId)
    {
        var userId = _loggedUserService.GetUserId();
        var store = await _writeOnlyRepository.GetByIdTracking(storeId) ?? throw new NotFoundException([ResourceMessagesException.STORE_NOT_FOUND]);

        if (userId != store.UserId)
            throw new UnauthorizedStoreException([ResourceMessagesException.UNAUTHORIZED_STORE]);

        await Validate(request, store.Name!, userId);

        request.Adapt(store);

        await _uof.Commit();

        return store.Adapt<StoreResponseDto>();
    }

    private async Task Validate(UpdateStoreRequestDto request, string storeName, string userId )
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
