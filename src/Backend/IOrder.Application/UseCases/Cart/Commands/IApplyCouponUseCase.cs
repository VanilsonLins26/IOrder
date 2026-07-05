using IOrder.Communication.Request;
using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Cart.Commands;

public interface IApplyCouponUseCase
{
    Task<CartResponseDto> Execute(ApplyCouponRequestDto request);
}
