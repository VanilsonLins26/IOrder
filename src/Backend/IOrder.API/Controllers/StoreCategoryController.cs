using IOrder.Application.UseCases.StoreCategory.Queries;
using IOrder.Communication.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOrder.API.Controllers;

[Route("[controller]")]
[ApiController]
public class StoreCategoryController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IList<StoreCategoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromServices] IGetAllStoreCategoryUseCase useCase)
    {
        var response = await useCase.Execute();
        return Ok(response);
    }
}

