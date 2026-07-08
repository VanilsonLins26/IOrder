using IOrder.Application.UseCases.Chat.Commands;
using IOrder.Application.UseCases.Chat.Queries;
using IOrder.Communication.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IOrder.API.Controllers;

[Authorize]
public class ChatController : IOrderBaseController
{
    [HttpGet("conversations")]
    [ProducesResponseType(typeof(PagedResponse<ConversationResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetConversations(
        [FromServices] IGetConversationsUseCase useCase,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var response = await useCase.Execute(pageNumber, pageSize);
        return Ok(response);
    }

    [HttpGet("{orderId:guid}/messages")]
    [ProducesResponseType(typeof(PagedResponse<OrderMessageResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMessages(
        [FromServices] IGetMessagesUseCase useCase,
        Guid orderId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var response = await useCase.Execute(orderId, pageNumber, pageSize);
        return Ok(response);
    }

    [HttpPost("{orderId:guid}/read")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAsRead(
        [FromServices] IMarkAsReadUseCase useCase,
        Guid orderId)
    {
        var unreadCount = await useCase.Execute(orderId);
        return Ok(new { unreadCount });
    }
}
