using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Product;

public interface IGetProductById
{
    Task<ProductResponseDto> Execute(Guid id);
}
