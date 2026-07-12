using IOrder.Application.UseCases.Payment.Commands;
using IOrder.Application.UseCases.Payment.Queries;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IOrder.API.Controllers;

[Authorize]
public class PaymentController : IOrderBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(PaymentResponseDto), StatusCodes.Status201Created)]
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

    [HttpPost("webhook")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ProcessWebhook(
        [FromServices] IProcessPaymentWebhookUseCase useCase)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();
        var signature = Request.Headers["X-Signature"].FirstOrDefault()
            ?? Request.Headers["x-signature"].FirstOrDefault();

        await useCase.Execute(payload, signature);
        return Ok();
    }
}
