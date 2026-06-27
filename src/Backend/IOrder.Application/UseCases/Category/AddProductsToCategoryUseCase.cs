using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Category;
using IOrder.Domain.Repositories.Product;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Category;

public class AddProductsToCategoryUseCase : IAddProductsToCategoryUseCase
{
    private readonly ICategoryWriteOnlyRepository _categoryWriteOnlyRepository;
    private readonly IProductWriteOnlyRepository _productWriteOnlyRepository;
    private readonly IStorePermissionService _permissionService;
    private readonly IUnitOfWork _uof;

    public AddProductsToCategoryUseCase(
        ICategoryWriteOnlyRepository categoryWriteOnlyRepository,
        IProductWriteOnlyRepository productWriteOnlyRepository,
        IStorePermissionService permissionService,
        IUnitOfWork uof)
    {
        _categoryWriteOnlyRepository = categoryWriteOnlyRepository;
        _productWriteOnlyRepository = productWriteOnlyRepository;
        _permissionService = permissionService;
        _uof = uof;
    }

    public async Task<CategoryResponseDto> Execute(Guid categoryId, AddProductsToCategoryRequestDto request)
    {
        if (request.ProductIds == null || !request.ProductIds.Any())
            throw new ErrorOnValidationException(["A lista de produtos não pode estar vazia."]);

        var category = await _categoryWriteOnlyRepository.GetByIdTracking(categoryId) ?? throw new NotFoundException([ResourceMessagesException.CATEGORY_NOT_FOUND]);
        
        await _permissionService.ValidateCategoryOwnershipAsync(category);

        var products = await _productWriteOnlyRepository.GetByIdsTracking(request.ProductIds);

        foreach (var product in products)
        {
            await _permissionService.ValidateProductOwnershipAsync(product);
            product.CategoryId = categoryId;
        }

        await _uof.Commit();

        return category.Adapt<CategoryResponseDto>();
    }
}
