using IOrder.Application.Services.LoggedUser;
using IOrder.Application.Services.StorePermission;
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

public class UpdateOpeningHourUseCase : IUpdateOpeningHourUseCase
{
    private readonly IStoreWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly ILoggedUserService _loggedUserService;
    private readonly IStorePermissionService _storePermissionService;

    public UpdateOpeningHourUseCase(IStoreWriteOnlyRepository writeOnlyRepository, IUnitOfWork uof, ILoggedUserService loggedUserService, IStorePermissionService storePermissionService)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _loggedUserService = loggedUserService;
        _storePermissionService = storePermissionService;
    }
    public async Task<StoreResponseDto> Execute(UpdateOpeningHourRequestDto request, Guid storeId)
    {
        var store = await _writeOnlyRepository.GetByIdTracking(storeId) ?? throw new NotFoundException([ResourceMessagesException.STORE_NOT_FOUND]);

        await _storePermissionService.ValidateStoreOwnerAsync(store);

        await Validate(request);

        var newHours = request.OpeningHours.Adapt<ICollection<OpeningHour>>();
        store.OpeningHours.Clear();

        foreach (var hours in newHours)
            store.OpeningHours.Add(hours);

        await _uof.Commit();

        return store.Adapt<StoreResponseDto>();
    }

    private async Task Validate(UpdateOpeningHourRequestDto request)
    {
        var validator = new UpdateOpeningHourValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }


    }
}
