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

        // Remove hours that are not in the new request
        var newDays = request.OpeningHours.Select(h => h.DayOfWeek).ToList();
        var hoursToRemove = store.OpeningHours.Where(h => !newDays.Contains(h.DayOfWeek)).ToList();
        foreach (var h in hoursToRemove)
        {
            store.OpeningHours.Remove(h);
            _writeOnlyRepository.DeleteOpeningHour(h); // We'll add this method
        }

        foreach (var reqHour in request.OpeningHours)
        {
            var existing = store.OpeningHours.FirstOrDefault(h => h.DayOfWeek == reqHour.DayOfWeek);
            if (existing != null)
            {
                existing.OpenHour = reqHour.OpenHour;
                existing.CloseHour = reqHour.CloseHour;
            }
            else
            {
                var newHour = new OpeningHour
                {
                    DayOfWeek = reqHour.DayOfWeek.Value,
                    OpenHour = reqHour.OpenHour,
                    CloseHour = reqHour.CloseHour,
                    StoreId = storeId
                };
                store.OpeningHours.Add(newHour);
                _writeOnlyRepository.AddOpeningHour(newHour);
            }
        }

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
