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
    private readonly IProductReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _uof;

    public DeleteProductUseCase(IProductWriteOnlyRepository writeOnlyRepository, IProductReadOnlyRepository readOnlyRepository, IUnitOfWork uof)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _uof = uof;
    }

    public async Task<ProductResponseDto> Execute(Guid id)
    {
        var produto = await _readOnlyRepository.GetByIdAsync(id) ?? throw new NotFoundException([ResourceMessagesException.PRODUCT_NOT_FOUND]);

        _writeOnlyRepository.Delete(produto);

        await _uof.Commit();

        return produto.Adapt<ProductResponseDto>();
    }
}
