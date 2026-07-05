using IOrder.Communication.Request;
using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Cart.Commands;

public interface IRemoveItemUseCase
{
    Task<CartResponseDto> Execute(Guid cartItemId);
}
