using IOrder.Domain.Security.Services;
using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Category;
using IOrder.Domain.Repositories.Product;
using IOrder.Domain.Repositories.Store;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Category.Commands;

public class DeleteCategoryUseCase : IDeleteCategoryUseCase
{
    private readonly ICategoryWriteOnlyRepository _writeOnlyRepository;
    private readonly IProductWriteOnlyRepository _productWriteOnlyRepository;
    private readonly IProductReadOnlyRepository _productReadOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly IStorePermissionService _storePermissionService;

    public DeleteCategoryUseCase(
        ICategoryWriteOnlyRepository writeOnlyRepository,
        IProductWriteOnlyRepository productWriteOnlyRepository,
        IProductReadOnlyRepository productReadOnlyRepository,
        IUnitOfWork uof,
        IStorePermissionService storePermissionService)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _productWriteOnlyRepository = productWriteOnlyRepository;
        _productReadOnlyRepository = productReadOnlyRepository;
        _uof = uof;
        _storePermissionService = storePermissionService;
    }

    public async Task<CategoryResponseDto> Execute(Guid id)
    {
        var category = await _writeOnlyRepository.GetByIdTracking(id) ?? throw new NotFoundException([ResourceMessagesException.CATEGORY_NOT_FOUND]);

        await _storePermissionService.ValidateCategoryOwnershipAsync(category);

        var products = (await _productReadOnlyRepository.GetAllAsync()).Where(p => p.CategoryId == id).ToList();

        if (products.Any())
        {
            var trackedProducts = await _productWriteOnlyRepository.GetByIdsTracking(products.Select(p => p.Id).ToList());
            foreach (var product in trackedProducts)
            {
                product.CategoryId = null;
            }
        }

        _writeOnlyRepository.Delete(category);

        await _uof.Commit();

        return category.Adapt<CategoryResponseDto>();
    }
}

