using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Category;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using System;
using System.Threading.Tasks;
using FluentValidation;

namespace IOrder.Application.UseCases.Category.Commands;

public class UpdateCategoryUseCase : IUpdateCategoryUseCase
{
    private readonly ICategoryReadOnlyRepository _readOnlyRepository;
    private readonly ICategoryWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly IStorePermissionService _permissionService;
    private readonly IValidator<CategoryRequestDto> _validator;

    public UpdateCategoryUseCase(ICategoryReadOnlyRepository readOnlyRepository, 
                                 ICategoryWriteOnlyRepository writeOnlyRepository, 
                                 IUnitOfWork uof,
                                 IStorePermissionService permissionService,
                                 IValidator<CategoryRequestDto> validator)
    {
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _permissionService = permissionService;
        _validator = validator;
    }

    public async Task<CategoryResponseDto> Execute(Guid id, CategoryRequestDto request)
    {
        var category = await _writeOnlyRepository.GetByIdTracking(id) ?? throw new NotFoundException([ResourceMessagesException.CATEGORY_NOT_FOUND]);

        await _permissionService.ValidateCategoryOwnershipAsync(category);

        await Validate(request, category);

        category.Name = request.Name;
        category.Position = request.Position.Value;

        _writeOnlyRepository.Update(category);
        await _uof.Commit();

        return category.Adapt<CategoryResponseDto>();
    }

    private async Task Validate(CategoryRequestDto request, Domain.Entities.Category category)
    {
        var result = await _validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }

        if (request.Name != category.Name)
        {
            var nameExists = await _readOnlyRepository.NameExists(request.Name, category.StoreId);
            if (nameExists)
                throw new ErrorOnValidationException([ResourceMessagesException.NAME_ALREADY_EXISTS]);
        }
    }
}

