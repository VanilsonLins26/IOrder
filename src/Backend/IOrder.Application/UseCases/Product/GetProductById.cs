using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Product;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Product;

public class GetProductById : IGetProductById
{
    private readonly IProductReadOnlyRepository _readOnlyRepository;

    public GetProductById(IProductReadOnlyRepository readOnlyRepository)
    {
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<ProductResponseDto> Execute(Guid id)
    {
        var product = await _readOnlyRepository.GetByIdAsync(id) ?? throw new NotFoundException([ResourceMessagesException.PRODUCT_NOT_FOUND]);

        return product.Adapt<ProductResponseDto>();

       
    }
}
