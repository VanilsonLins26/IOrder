using IOrder.Domain.Security.Services;
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
using FluentValidation;

namespace IOrder.Application.UseCases.Store.Commands;

public class CreateStoreUseCase : ICreateStoreUseCase
{
    private readonly IStoreReadOnlyRepository _readOnlyRepository;
    private readonly IStoreWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly ILoggedUserService _loggedUserService;
    private readonly IValidator<StoreRequestDto> _validator;

    public CreateStoreUseCase(IStoreReadOnlyRepository readOnlyRepository, IStoreWriteOnlyRepository writeOnlyRepository, IUnitOfWork uof, ILoggedUserService loggedUserService, IValidator<StoreRequestDto> validator)
    {
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _loggedUserService = loggedUserService;
        _validator = validator;
    }

    public async Task<StoreResponseDto> Execute(StoreRequestDto request)
    {
        var userId = _loggedUserService.GetUserId();
        await Validate(request, userId);

        var store = request.Adapt<Domain.Entities.Store>();
        store.UserId = userId;
        store.AddDomainEvent(new IOrder.Domain.Events.StoreCreatedEvent(store.Id, store.Name));

        var createdStore = await _writeOnlyRepository.Create(store);

        await _uof.Commit();

        return createdStore.Adapt<StoreResponseDto>();
    }

    private async Task Validate(StoreRequestDto request, string userId)
    {
        var result = await _validator.ValidateAsync(request);

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



