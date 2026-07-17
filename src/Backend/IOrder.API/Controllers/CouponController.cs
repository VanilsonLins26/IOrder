using IOrder.Application.UseCases.Coupon.Commands;
using IOrder.Application.UseCases.Coupon.Queries;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IOrder.API.Controllers;

public class CouponController : IOrderBaseController
{
    [Authorize(Roles = "ShopKeeper")]
    [HttpPost]
    [ProducesResponseType(typeof(CouponResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromServices] ICreateCouponUseCase useCase,
        [FromBody] CouponRequestDto request)
    {
        var response = await useCase.Execute(request);
        return Created(string.Empty, response);
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(IList<CouponResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(
        [FromServices] IGetActiveCouponsUseCase useCase)
    {
        var response = await useCase.Execute();
        return Ok(response);
    }

    [Authorize(Roles = "ShopKeeper")]
    [HttpGet("admin")]
    [ProducesResponseType(typeof(IList<CouponResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromServices] IGetAllCouponsUseCase useCase)
    {
        var response = await useCase.Execute();
        return Ok(response);
    }
}
