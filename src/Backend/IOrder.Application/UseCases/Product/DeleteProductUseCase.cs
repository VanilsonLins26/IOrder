using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Product;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Product;

public class DeleteProductUseCase : IDeleteProductUseCase
{
    private readonly IProductWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly IStorePermissionService _storePermissionService;

    public DeleteProductUseCase(IProductWriteOnlyRepository writeOnlyRepository, IUnitOfWork uof, IStorePermissionService storePermissionService)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _storePermissionService = storePermissionService;
    }

    public async Task<ProductResponseDto> Execute(Guid id)
    {
        var produto = await _writeOnlyRepository.GetByIdTracking(id) ?? throw new NotFoundException([ResourceMessagesException.PRODUCT_NOT_FOUND]);

        await _storePermissionService.ValidateProductOwnershipAsync(produto);

        _writeOnlyRepository.Delete(produto);

        await _uof.Commit();

        return produto.Adapt<ProductResponseDto>();
    }
}
