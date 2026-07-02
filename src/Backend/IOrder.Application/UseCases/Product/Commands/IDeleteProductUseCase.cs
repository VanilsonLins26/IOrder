using IOrder.Communication.Request;
using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Product.Commands;

public interface IDeleteProductUseCase
{
    Task<ProductResponseDto> Execute(Guid id);
}

