using IOrder.Communication.Request;
using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Product;

public interface IDeleteProductUseCase
{
    Task<ProductResponseDto> Execute(Guid id);
}
