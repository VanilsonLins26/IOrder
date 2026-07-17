using IOrder.Communication.Request;
using IOrder.Communication.Response;


namespace IOrder.Application.UseCases.Product.Queries;

public interface IGetProductsPagedUseCase
{
    Task<PagedResponse<ProductResponseDto>> Execute(ProductSearchRequestDto query);
}

