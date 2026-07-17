using IOrder.Application.UseCases.Store.Commands;
using IOrder.Application.UseCases.Store.Queries;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Security.Services;
using IOrder.Domain.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
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
    [ProducesResponseType(typeof(PagedResponse<StoreResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromServices] IGetAllStoreUseCase useCase, [FromQuery] StoreSearchRequestDto request)
    {
        var store = await useCase.Execute(request);

        return Ok(store);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StoreResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(
        [FromServices] IGetByIdStoreUseCase usecase,
        Guid id,
        [FromQuery] double? userLatitude,
        [FromQuery] double? userLongitude)
    {
        var store = await usecase.Execute(id, userLatitude, userLongitude);

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

    [Authorize(Roles = "ShopKeeper")]
    [HttpPut("image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> UpdateImage(
    [FromServices] IUpdateStoreImageUseCase usecase,
    IFormFile file) 
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ResponseErrorDto("Nenhuma imagem foi enviada."));
        }

        using var stream = file.OpenReadStream();

        var imageUrl = await usecase.Execute(stream, file.FileName);

        return Ok(new { ImageUrl = imageUrl });
    }

    [Authorize(Roles = "ShopKeeper")]
    [HttpPost("geocode-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GeocodeAllStores(
        [FromServices] IGeocodingService geocodingService,
        [FromServices] IStoreReadOnlyRepository readRepository,
        [FromServices] IStoreWriteOnlyRepository writeRepository,
        [FromServices] IUnitOfWork unitOfWork,
        [FromServices] ILoggedUserService loggedUserService)
    {
        var myStore = await readRepository.GetByUserIdAsync(loggedUserService.GetUserId());
        if (myStore == null)
            return BadRequest(new ResponseErrorDto("Loja não encontrada."));

        if (myStore.Location != null)
            return Ok(new { Message = "Loja já possui localização.", Updated = false });

        if (myStore.Address == null)
            return BadRequest(new ResponseErrorDto("Loja não possui endereço."));

        var coords = await geocodingService.GetCoordinatesAsync(
            myStore.Address.Street, myStore.Address.City, myStore.Address.State, myStore.Address.ZipCode);
        
        if (coords.HasValue)
        {
            var trackedStore = await writeRepository.GetByIdTracking(myStore.Id);
            if (trackedStore != null)
            {
                trackedStore.Location = new NetTopologySuite.Geometries.Point(coords.Value.Latitude, coords.Value.Longitude) { SRID = 4326 };
                await unitOfWork.Commit();
            }
            return Ok(new { Message = "Localização atualizada.", Updated = true });
        }

        return Ok(new { Message = "Não foi possível geocodificar o endereço.", Updated = false });
    }

}

