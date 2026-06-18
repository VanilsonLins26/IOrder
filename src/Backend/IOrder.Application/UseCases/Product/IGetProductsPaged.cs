using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.SeedWork.Pagination;

namespace IOrder.Application.UseCases.Product;

public interface IGetProductsPaged
{
    Task<IEnumerable<ProductResponseDto>> Execute(ProductSearchQuery query);
}
