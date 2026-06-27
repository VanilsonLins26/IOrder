using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Category;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Category;

public class CreateCategoryUseCase : ICreateCategoryUseCase
{
    private readonly ICategoryReadOnlyRepository _readOnlyRepository;
    private readonly ICategoryWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly IStorePermissionService _permissionService;

    public CreateCategoryUseCase(ICategoryReadOnlyRepository readOnlyRepository, 
                                 ICategoryWriteOnlyRepository writeOnlyRepository, IUnitOfWork uof,
                                 IStorePermissionService permissionService)
    {
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _permissionService = permissionService;
    }

    public async Task<CategoryResponseDto> Execute(CategoryRequestDto request)
    {
        var storeId = await _permissionService.GetLoggedUserStoreIdAsync();

        await Validate(request, storeId);

        var category = request.Adapt<Domain.Entities.Category>();
        category.StoreId = storeId;

        var createdCategory = await _writeOnlyRepository.Create(category);

        await _uof.Commit();

        return createdCategory.Adapt<CategoryResponseDto>();
    }

    private async Task Validate(CategoryRequestDto request, Guid storeId)
    {
        
        var validator = new CreateCategoryValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }

        var nameExists = await _readOnlyRepository.NameExists(request.Name, storeId);


        if (nameExists)
            throw new ErrorOnValidationException([ResourceMessagesException.NAME_ALREADY_EXISTS]);

    }
}
