using IOrder.Application.UseCases.Profile.Commands;
using IOrder.Application.UseCases.Profile.Queries;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IOrder.API.Controllers;

[Authorize]
public class ProfileController : IOrderBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(UserProfileResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromServices] IGetUserProfileUseCase useCase)
    {
        var response = await useCase.Execute();
        return Ok(response);
    }

    [HttpPut]
    [ProducesResponseType(typeof(UserProfileResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        [FromServices] IUpdateUserProfileUseCase useCase,
        [FromBody] UserProfileRequestDto request)
    {
        var response = await useCase.Execute(request);
        return Ok(response);
    }
}
