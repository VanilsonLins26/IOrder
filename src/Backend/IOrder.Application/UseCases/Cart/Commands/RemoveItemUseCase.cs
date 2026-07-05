using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Cart;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Cart.Commands;

public class RemoveItemUseCase : IRemoveItemUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly ICartReadOnlyRepository _readOnlyRepositoy;
    private readonly ICartWriteOnlyRepository _writeOnlyRepository;

    public RemoveItemUseCase(
        ILoggedUserService loggedUserService, 
        ICartReadOnlyRepository readOnlyRepositoy, 
        ICartWriteOnlyRepository writeOnlyRepository)
    {
        _loggedUserService = loggedUserService;
        _readOnlyRepositoy = readOnlyRepositoy;
        _writeOnlyRepository = writeOnlyRepository;
    }

    public async Task<CartResponseDto> Execute(Guid cartItemId)
    {
        var userId = _loggedUserService.GetUserId();

        var cart = await _readOnlyRepositoy.GetCartAsync(userId) ?? throw new NotFoundException([ResourceMessagesException.INVALID_CART]);

        var result = cart.RemoveCartItem(cartItemId);
        if (!result)
        {
            throw new NotFoundException([ResourceMessagesException.CART_ITEM_NOT_FOUND]);
        }

        await _writeOnlyRepository.SaveCartAsync(cart);

        return cart.Adapt<CartResponseDto>();
    }
}
