using IOrder.Application.UseCases.UserCard.Commands;
using IOrder.Application.UseCases.UserCard.Queries;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IOrder.API.Controllers;

[Authorize]
public class UserCardController : IOrderBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(UserCardResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveCard(
        [FromServices] ISaveUserCardUseCase useCase,
        [FromBody] SaveCardRequestDto request)
    {
        var response = await useCase.Execute(request);
        return Created(string.Empty, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IList<UserCardResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCards(
        [FromServices] IGetUserCardsUseCase useCase)
    {
        var response = await useCase.Execute();
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCard(
        [FromServices] IDeleteUserCardUseCase useCase,
        [FromRoute] Guid id)
    {
        await useCase.Execute(id);
        return NoContent();
    }
}
