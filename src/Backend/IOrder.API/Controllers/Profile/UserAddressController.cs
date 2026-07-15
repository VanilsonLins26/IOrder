using IOrder.Application.UseCases.Profile.Commands;
using IOrder.Application.UseCases.Profile.Queries;
using IOrder.Communication.Request.Profile;
using IOrder.Communication.Response.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IOrder.API.Controllers.Profile;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserAddressController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(UserAddressResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Add(
        [FromBody] AddUserAddressRequestDto request,
        [FromServices] IAddUserAddressUseCase useCase)
    {
        var response = await useCase.ExecuteAsync(request);
        return Created(string.Empty, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<UserAddressResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromServices] IGetUserAddressesUseCase useCase)
    {
        var response = await useCase.ExecuteAsync();
        return Ok(response);
    }

    [HttpPut("{id:guid}/default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetDefault(
        [FromRoute] Guid id,
        [FromServices] ISetDefaultAddressUseCase useCase)
    {
        await useCase.ExecuteAsync(id);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        [FromServices] IDeleteUserAddressUseCase useCase)
    {
        await useCase.ExecuteAsync(id);
        return NoContent();
    }
}
