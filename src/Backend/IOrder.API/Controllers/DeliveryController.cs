using IOrder.Application.UseCases.Delivery.Commands;
using IOrder.Application.UseCases.Delivery.Queries;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.infrastructure.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace IOrder.API.Controllers;

[Authorize]
public class DeliveryController : IOrderBaseController
{
    [HttpPost("orders/{orderId:guid}/assign")]
    [ProducesResponseType(typeof(DeliveryAssignmentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignCourier(
        [FromServices] IAssignDeliveryUseCase useCase,
        [FromServices] IHubContext<ChatHub> hubContext,
        [FromRoute] Guid orderId,
        [FromBody] AssignCourierRequestDto request)
    {
        var response = await useCase.Execute(orderId, request);

        await hubContext.Clients.Group(orderId.ToString()).SendAsync(
            "CourierAssigned",
            new { OrderId = orderId, CourierUserId = request.CourierUserId });

        return Created(string.Empty, response);
    }

    [HttpPatch("assignments/{assignmentId:guid}/accept")]
    [ProducesResponseType(typeof(DeliveryAssignmentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptDelivery(
        [FromServices] IAcceptDeliveryUseCase useCase,
        [FromServices] IHubContext<ChatHub> hubContext,
        [FromRoute] Guid assignmentId)
    {
        var response = await useCase.Execute(assignmentId);

        await hubContext.Clients.Group(response.OrderId.ToString()).SendAsync(
            "OrderStatusChanged",
            new { OrderId = response.OrderId, Status = (int)4 });

        return Ok(response);
    }

    [HttpPatch("assignments/{assignmentId:guid}/reject")]
    [ProducesResponseType(typeof(DeliveryAssignmentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectDelivery(
        [FromServices] IRejectDeliveryUseCase useCase,
        [FromRoute] Guid assignmentId)
    {
        var response = await useCase.Execute(assignmentId);
        return Ok(response);
    }

    [HttpPatch("assignments/{assignmentId:guid}/pickup")]
    [ProducesResponseType(typeof(DeliveryAssignmentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PickupOrder(
        [FromServices] IPickupOrderUseCase useCase,
        [FromServices] IHubContext<ChatHub> hubContext,
        [FromRoute] Guid assignmentId)
    {
        var response = await useCase.Execute(assignmentId);

        await hubContext.Clients.Group(response.OrderId.ToString()).SendAsync(
            "OrderStatusChanged",
            new { OrderId = response.OrderId, Status = (int)5 });

        return Ok(response);
    }

    [HttpPatch("assignments/{assignmentId:guid}/deliver")]
    [ProducesResponseType(typeof(DeliveryAssignmentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeliverOrder(
        [FromServices] IDeliverOrderUseCase useCase,
        [FromServices] IHubContext<ChatHub> hubContext,
        [FromRoute] Guid assignmentId)
    {
        var response = await useCase.Execute(assignmentId);

        await hubContext.Clients.Group(response.OrderId.ToString()).SendAsync(
            "OrderStatusChanged",
            new { OrderId = response.OrderId, Status = (int)6 });

        return Ok(response);
    }

    [HttpPut("location")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateLocation(
        [FromServices] IUpdateCourierLocationUseCase useCase,
        [FromBody] UpdateCourierLocationRequestDto request)
    {
        await useCase.Execute(request);
        return NoContent();
    }

    [HttpGet("courier/{courierUserId}/location")]
    [ProducesResponseType(typeof(CourierLocationResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourierLocation(
        [FromServices] IGetCourierLocationUseCase useCase,
        [FromRoute] string courierUserId)
    {
        var response = await useCase.Execute(courierUserId);
        return Ok(response);
    }

    [HttpGet("my-deliveries")]
    [ProducesResponseType(typeof(PagedResponse<DeliveryAssignmentResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyDeliveries(
        [FromServices] IGetMyDeliveriesUseCase useCase,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await useCase.Execute(pageNumber, pageSize);
        return Ok(response);
    }
}
