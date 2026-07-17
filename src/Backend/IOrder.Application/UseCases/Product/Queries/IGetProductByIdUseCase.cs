using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Product.Queries;

public interface IGetProductByIdUseCase
{
    Task<ProductResponseDto> Execute(Guid id);
}

