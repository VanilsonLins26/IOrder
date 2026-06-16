using IOrder.Communication.Request;
using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Product;

public interface ICreatePromotionPriceUseCase
{
    Task<PromotionPriceResponse> Execute(PromotionPriceResquestDto dto);
}
