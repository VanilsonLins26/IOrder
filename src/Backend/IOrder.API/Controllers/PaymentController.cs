using IOrder.Application.UseCases.Payment.Commands;
using IOrder.Application.UseCases.Payment.Queries;
using IOrder.Application.UseCases.UserCard.Commands;
using IOrder.Application.UseCases.UserCard.Queries;
using IOrder.Application.UseCases.Payment.Queries;
using IOrder.Communication.Enums;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.infrastructure.Hubs;
using IOrder.infrastructure.Services.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace IOrder.API.Controllers;

[Authorize]
public class PaymentController : IOrderBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(PaymentIntentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePayment(
        [FromServices] ICreatePaymentUseCase useCase,
        [FromBody] CreatePaymentRequestDto request)
    {
        var response = await useCase.Execute(request);

        return Created(string.Empty, response);
    }

    [HttpGet("{orderId:guid}")]
    [ProducesResponseType(typeof(PaymentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentByOrder(
        [FromServices] IGetPaymentByOrderUseCase useCase,
        Guid orderId)
    {
        var response = await useCase.Execute(orderId);
        if (response is null)
            return NotFound();

        return Ok(response);
    }

    [HttpGet("public-key")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PublicKeyResponseDto), StatusCodes.Status200OK)]
    public IActionResult GetPublicKey(
        [FromServices] IOptions<StripeSettings> settings)
    {
        return Ok(new PublicKeyResponseDto { PublicKey = settings.Value.PublishableKey });
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ProcessWebhook(
        [FromServices] IProcessPaymentWebhookUseCase useCase,
        [FromServices] IHubContext<ChatHub> hubContext)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();

        var response = await useCase.Execute(payload, signature);

        if (response?.Status is PaymentStatusDto.Approved or PaymentStatusDto.Rejected)
        {
            await hubContext.Clients.Group(response.OrderId.ToString()).SendAsync(
                "PaymentStatusChanged",
                new
                {
                    OrderId = response.OrderId,
                    PaymentId = response.Id,
                    Status = response.Status,
                    PaidAt = response.PaidAt
                });
        }

        return Ok();
    }

    [HttpGet("cards")]
    [ProducesResponseType(typeof(IList<UserCardResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCards(
        [FromServices] IGetUserCardsUseCase useCase)
    {
        var response = await useCase.Execute();
        return Ok(response);
    }

    [HttpDelete("cards/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCard(
        [FromServices] IDeleteUserCardUseCase useCase,
        [FromRoute] string id)
    {
        await useCase.Execute(id);
        return NoContent();
    }

    [HttpPatch("{orderId:guid}/save-card")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSaveCard(
        [FromServices] IUpdatePaymentIntentSaveCardUseCase useCase,
        [FromRoute] Guid orderId,
        [FromBody] UpdateSaveCardRequestDto request)
    {
        await useCase.Execute(orderId, request.SaveCard);
        return Ok();
    }
}
