using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Category;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using System;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Category;

public class GetCategoryByIdUseCase : IGetCategoryByIdUseCase
{
    private readonly ICategoryReadOnlyRepository _readOnlyRepository;
    private readonly IStorePermissionService _permissionService;

    public GetCategoryByIdUseCase(ICategoryReadOnlyRepository readOnlyRepository, IStorePermissionService permissionService)
    {
        _readOnlyRepository = readOnlyRepository;
        _permissionService = permissionService;
    }

    public async Task<CategoryResponseDto> Execute(Guid id)
    {
        var category = await _readOnlyRepository.GetByIdAsync(id) ?? throw new NotFoundException([ResourceMessagesException.CATEGORY_NOT_FOUND]);
        
        await _permissionService.ValidateCategoryOwnershipAsync(category);

        return category.Adapt<CategoryResponseDto>();
    }
}
