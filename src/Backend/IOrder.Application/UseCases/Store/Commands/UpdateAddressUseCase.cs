using IOrder.Domain.Security.Services;
using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace IOrder.Application.UseCases.Store.Commands;

public class UpdateAddressUseCase : IUpdateAddressUseCase
{
    private readonly IStoreWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly ILoggedUserService _loggedUserService;
    private readonly IStorePermissionService _storePermissionService;
    private readonly IValidator<AddressRequestDto> _validator;
    private readonly IGeocodingService _geocodingService;

    public UpdateAddressUseCase(
        IStoreWriteOnlyRepository writeOnlyRepository,
        IUnitOfWork uof,
        ILoggedUserService loggedUserService,
        IStorePermissionService storePermissionService,
        IValidator<AddressRequestDto> validator,
        IGeocodingService geocodingService)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _loggedUserService = loggedUserService;
        _storePermissionService = storePermissionService;
        _validator = validator;
        _geocodingService = geocodingService;
    }

    public async Task<StoreResponseDto> Execute(AddressRequestDto request, Guid storeId)
    {
        var store = await _writeOnlyRepository.GetByIdTracking(storeId) ?? throw new NotFoundException([ResourceMessagesException.STORE_NOT_FOUND]);

        await _storePermissionService.ValidateStoreOwnerAsync(storeId);

        await Validate(request);

        var updatedAddress = request.Adapt<Domain.Entities.Address>();
        store.Address = updatedAddress;

        var coords = await _geocodingService.GetCoordinatesAsync(
            request.Street, request.City, request.State, request.ZipCode);
        if (coords.HasValue)
        {
            store.Location = new NetTopologySuite.Geometries.Point(coords.Value.Longitude, coords.Value.Latitude) { SRID = 4326 };
        }

        await _uof.Commit();

        return store.Adapt<StoreResponseDto>();
    }

    private async Task Validate(AddressRequestDto request)
    {
        var result = await _validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }


    }
}

