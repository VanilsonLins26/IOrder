using IOrder.Application.UseCases.Store;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Pagination;
using IOrder.Domain.SeedWork.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IOrder.API.Controllers;


public class StoreController : IOrderBaseController
{
    [Authorize(Roles = "ShopKeeper")]
    [HttpPost]
    [ProducesResponseType(typeof(StoreResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromServices] ICreateStoreUseCase useCase, [FromBody] StoreRequestDto storeRequest)
    {
        var response = await useCase.Execute(storeRequest);

        return Created(string.Empty, response);
    }

    [Authorize(Roles = "ShopKeeper")]
    [HttpDelete("{storeId:guid}")]
    [ProducesResponseType(typeof(StoreResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid storeId, [FromServices] IDeleteStoreUseCase useCase)
    {
        var deletedStore = await useCase.Execute(storeId);

        return Ok(deletedStore);
    }

    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedList<StoreResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromServices] IGetAllStore useCase, [FromQuery] StoreSearchQuery query)
    {
        var store = await useCase.Execute(query);

        return Ok(store);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StoreResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById([FromServices] IGetByIdStoreUseCase usecase, Guid id)
    {
        var store = await usecase.Execute(id);

        return Ok(store);
    }

    [Authorize(Roles = "ShopKeeper")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(StoreResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> Update([FromServices] IUpdateStoreUseCase usecase, Guid id, [FromBody] UpdateStoreRequestDto dto)
    {
        var store = await usecase.Execute(dto, id);

        return Ok(store);
    }

    [Authorize(Roles = "ShopKeeper")]
    [HttpGet("myStore")]
    [ProducesResponseType(typeof(StoreResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetMyStore([FromServices] IGetMyStoreUseCase useCase)
    {
        var store = await useCase.Execute();

        return Ok(store);
    }

    [Authorize(Roles = "ShopKeeper")]
    [HttpPut("openingHour/{id:guid}")]
    [ProducesResponseType(typeof(StoreResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> UpdateOpeningHour([FromServices] IUpdateOpeningHourUseCase usecase, Guid id, [FromBody] UpdateOpeningHourRequestDto dto)
    {
        var store = await usecase.Execute(dto, id);

        return Ok(store);
    }

    [Authorize(Roles = "ShopKeeper")]
    [HttpPut("address/{id:guid}")]
    [ProducesResponseType(typeof(StoreResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> UpdateAddress([FromServices] IUpdateAddressUseCase usecase, Guid id, [FromBody] AddressRequestDto dto)
    {
        var store = await usecase.Execute(dto, id);

        return Ok(store);
    }


}
