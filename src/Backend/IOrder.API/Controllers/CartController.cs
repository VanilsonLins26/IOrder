using IOrder.Application.UseCases.Cart.Commands;
using IOrder.Application.UseCases.Cart.Queries;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IOrder.API.Controllers;


[Authorize]
public class CartController : IOrderBaseController
{

    [HttpGet]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCart([FromServices] IGetCartUseCase useCase)
    {
        var cartResponse = await useCase.Execute();

        return Ok(cartResponse);

    }

    [HttpPatch("AddItem")]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> AddItemToCart([FromServices] IAddItemToCartUseCase useCase, [FromBody] AddItemToCartRequestDto request)
    {
        var cartResponse = await useCase.Execute(request);

        return Ok(cartResponse);

    }

    [HttpPatch("Coupon")]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> AddCoupon([FromServices] IApplyCouponUseCase useCase, [FromBody] ApplyCouponRequestDto request)
    {
        var cartResponse = await useCase.Execute(request);

        return Ok(cartResponse);

    }

    [HttpPatch("ChangeQuantity")]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ChangeItemQuantity([FromServices] IChangeQuantityUseCase useCase, [FromBody] ChangeCartItemQuantityRequestDto request)
    {
        var cartResponse = await useCase.Execute(request);

        return Ok(cartResponse);

    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> ClearCart([FromServices] IClearCartUseCase useCase)
    {
        await useCase.Execute();

        return Ok();

    }

    [HttpDelete("{cartItemId:guid}")]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveItemFromCart([FromServices] IRemoveItemUseCase useCase, Guid cartItemId)
    {
        var response = await useCase.Execute(cartItemId);

        return Ok(response);

    }

}
