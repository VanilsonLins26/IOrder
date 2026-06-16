using IOrder.Communication.Request;
using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Product;

public interface IUpdateProductUseCase
{
    Task<ProductResponseDto> Execute(Guid id, UpdateProductRequestDto dto);
}
