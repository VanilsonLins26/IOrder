using IOrder.Application.UseCases.Customization.Commands;
using IOrder.Application.UseCases.Customization.Queries;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IOrder.API.Controllers;

[Authorize]
public class CustomizationController : IOrderBaseController
{

    [HttpGet("product/{productId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CustomizationGroupResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetByProduct(
        [FromServices] IGetCustomizationsUseCase useCase,
        Guid productId)
    {
        var result = await useCase.Execute(productId);
        return Ok(result);
    }

    [HttpPost("product/{productId:guid}")]
    [ProducesResponseType(typeof(CustomizationGroupResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateGroup(
        [FromServices] ISaveCustomizationGroupUseCase useCase,
        Guid productId,
        [FromBody] SaveCustomizationGroupRequestDto request)
    {
        var result = await useCase.Execute(productId, request);
        return CreatedAtAction(nameof(GetByProduct), new { productId }, result);
    }

    [HttpDelete("{groupId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteGroup(
        [FromServices] IDeleteCustomizationGroupUseCase useCase,
        Guid groupId)
    {
        await useCase.Execute(groupId);
        return Ok();
    }

}
