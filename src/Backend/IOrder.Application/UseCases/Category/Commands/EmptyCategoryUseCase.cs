using IOrder.Application.Services.StorePermission;
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

namespace IOrder.Application.UseCases.Category.Commands;

public class EmptyCategoryUseCase : IEmptyCategoryUseCase
{
    private readonly ICategoryWriteOnlyRepository _categoryWriteOnlyRepository;
    private readonly IProductWriteOnlyRepository _productWriteOnlyRepository;
    private readonly IProductReadOnlyRepository _productReadOnlyRepository;
    private readonly IStorePermissionService _permissionService;
    private readonly IUnitOfWork _uof;

    public EmptyCategoryUseCase(
        ICategoryWriteOnlyRepository categoryWriteOnlyRepository,
        IProductWriteOnlyRepository productWriteOnlyRepository,
        IProductReadOnlyRepository productReadOnlyRepository,
        IStorePermissionService permissionService,
        IUnitOfWork uof)
    {
        _categoryWriteOnlyRepository = categoryWriteOnlyRepository;
        _productWriteOnlyRepository = productWriteOnlyRepository;
        _productReadOnlyRepository = productReadOnlyRepository;
        _permissionService = permissionService;
        _uof = uof;
    }

    public async Task<CategoryResponseDto> Execute(Guid categoryId)
    {
        var category = await _categoryWriteOnlyRepository.GetByIdTracking(categoryId) ?? throw new NotFoundException([ResourceMessagesException.CATEGORY_NOT_FOUND]);
        
        await _permissionService.ValidateCategoryOwnershipAsync(category);

        var products = _productReadOnlyRepository.GetAll().Where(p => p.CategoryId == categoryId).ToList();

        if (products.Any())
        {
            var trackedProducts = await _productWriteOnlyRepository.GetByIdsTracking(products.Select(p => p.Id).ToList());
            foreach (var product in trackedProducts)
            {
                product.CategoryId = null;
            }
        }

        await _uof.Commit();

        return category.Adapt<CategoryResponseDto>();
    }
}

