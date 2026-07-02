using IOrder.Communication.Request;
using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Product.Commands;

public interface ICreatePromotionPriceUseCase
{
    Task<PromotionPriceResponseDto> Execute(PromotionPriceResquestDto dto);
}

