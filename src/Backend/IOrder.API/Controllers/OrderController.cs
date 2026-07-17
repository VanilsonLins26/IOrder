using IOrder.Application.UseCases.Delivery.Commands;
using IOrder.Application.UseCases.Order.Commands;
using IOrder.Application.UseCases.Order.Queries;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.infrastructure.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace IOrder.API.Controllers;

[Authorize]
public class OrderController : IOrderBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateOrder(
        [FromServices] ICreateOrderUseCase useCase,
        [FromBody] CreateOrderRequestDto request)
    {
        var response = await useCase.Execute(request);
        return Created(string.Empty, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(
        [FromServices] IGetOrderByIdUseCase useCase,
        Guid id)
    {
        var response = await useCase.Execute(id);
        return Ok(response);
    }

    [HttpGet("user")]
    [ProducesResponseType(typeof(PagedResponse<OrderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserOrders(
        [FromServices] IGetOrdersByUserUseCase useCase,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await useCase.Execute(pageNumber, pageSize);
        return Ok(response);
    }

    [HttpGet("store")]
    [ProducesResponseType(typeof(PagedResponse<OrderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStoreOrders(
        [FromServices] IGetOrdersByStoreUseCase useCase,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await useCase.Execute(pageNumber, pageSize);
        return Ok(response);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        [FromServices] IUpdateOrderStatusUseCase useCase,
        [FromServices] IHubContext<ChatHub> hubContext,
        [FromRoute] Guid id,
        [FromBody] UpdateOrderStatusRequestDto request)
    {
        var response = await useCase.Execute(id, request);

        await hubContext.Clients.Group(id.ToString()).SendAsync(
            "OrderStatusChanged",
            new
            {
                OrderId = response.Id,
                Status = (int)response.Status
            });

        return Ok(response);
    }

    [HttpPatch("{id:guid}/negotiate")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> NegotiateOrder(
        [FromServices] INegotiateOrderUseCase useCase,
        [FromServices] IHubContext<ChatHub> hubContext,
        Guid id,
        [FromBody] NegotiateOrderRequestDto request)
    {
        var response = await useCase.Execute(id, request);

        await hubContext.Clients.Group(id.ToString()).SendAsync(
            "OrderStatusChanged",
            new
            {
                OrderId = response.Id,
                Status = (int)response.Status
            });

        return Ok(response);
    }

    [HttpPost("{id:guid}/message")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendMessage(
        [FromServices] ISendOrderMessageUseCase useCase,
        Guid id,
        [FromBody] SendOrderMessageRequestDto request)
    {
        var response = await useCase.Execute(id, request);
        return Ok(response);
    }

    [HttpPatch("{id:guid}/out-for-delivery")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsOutForDelivery(
        [FromServices] IMarkAsOutForDeliveryUseCase useCase,
        [FromServices] IHubContext<ChatHub> hubContext,
        [FromRoute] Guid id)
    {
        var response = await useCase.Execute(id);

        await hubContext.Clients.Group(id.ToString()).SendAsync(
            "OrderStatusChanged",
            new { OrderId = response.Id, Status = (int)response.Status });

        return Ok(response);
    }

    [HttpPatch("{id:guid}/request-early-delivery")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RequestEarlyDelivery(
        [FromServices] IRequestEarlyDeliveryUseCase useCase,
        [FromServices] IHubContext<ChatHub> hubContext,
        [FromRoute] Guid id)
    {
        var response = await useCase.Execute(id);

        await hubContext.Clients.Group(id.ToString()).SendAsync(
            "EarlyDeliveryRequested",
            new { OrderId = response.Id });

        return Ok(response);
    }
}
