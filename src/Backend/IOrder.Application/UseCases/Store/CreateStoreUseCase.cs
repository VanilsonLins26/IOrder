using IOrder.Application.Services.LoggedUser;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Store;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store;

public class CreateStoreUseCase : ICreateStoreUseCase
{
    private readonly IStoreReadOnlyRepository _readOnlyRepository;
    private readonly IStoreWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly ILoggedUserService _loggedUserService;

    public CreateStoreUseCase(IStoreReadOnlyRepository readOnlyRepository, IStoreWriteOnlyRepository writeOnlyRepository, IUnitOfWork uof, ILoggedUserService loggedUserService)
    {
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _loggedUserService = loggedUserService;
    }

    public async Task<StoreResponseDto> Execute(StoreRequestDto request)
    {
        var userId = _loggedUserService.GetUserId();
        await Validate(request, userId);

        var store = request.Adapt<Domain.Entities.Store>();
        store.UserId = userId;

        var createdStore = await _writeOnlyRepository.Create(store);

        await _uof.Commit();

        return createdStore.Adapt<StoreResponseDto>();
    }

    private async Task Validate(StoreRequestDto request, string userId)
    {
        var validator = new CreateStoreValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }

        var errorMessagesDb = new List<string>();

        var nameExists = await _readOnlyRepository.NameExists(request.Name!);

        if (nameExists)
            errorMessagesDb.Add(ResourceMessagesException.NAME_ALREADY_EXISTS);

        var userAlreadyHasStore = await _readOnlyRepository.HasStore(userId);

        if (userAlreadyHasStore)
            errorMessagesDb.Add(ResourceMessagesException.USER_ALREADY_HAS_STORE);     
        
        if (errorMessagesDb.Count > 0)
            throw new ErrorOnValidationException(errorMessagesDb);

    }

}


