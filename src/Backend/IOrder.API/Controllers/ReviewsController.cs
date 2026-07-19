using IOrder.Application.UseCases.Review.Commands;
using IOrder.Application.UseCases.Review.Queries;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IOrder.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    [HttpPost("{orderId}")]
    [Authorize]
    [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateReview(
        [FromRoute] Guid orderId,
        [FromBody] CreateReviewRequestDto request,
        [FromServices] ICreateReviewUseCase useCase)
    {
        var response = await useCase.ExecuteAsync(orderId, request);
        return Created(string.Empty, response);
    }

    [HttpGet("store/{storeId}")]
    [ProducesResponseType(typeof(IList<ReviewResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStoreReviews(
        [FromRoute] Guid storeId,
        [FromServices] IGetStoreReviewsUseCase useCase,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var response = await useCase.ExecuteAsync(storeId, page, pageSize);
        return Ok(response);
    }

    [HttpGet("order/{orderId}")]
    [Authorize]
    [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetReviewByOrder(
        [FromRoute] Guid orderId,
        [FromServices] IGetReviewByOrderIdUseCase useCase)
    {
        var response = await useCase.ExecuteAsync(orderId);
        
        if (response == null)
            return NoContent();
            
        return Ok(response);
    }
}
