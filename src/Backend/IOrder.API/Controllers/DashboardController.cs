using IOrder.Application.UseCases.Dashboard.Queries;
using IOrder.Communication.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IOrder.API.Controllers;

[Authorize(Roles = "ShopKeeper")]
public class DashboardController : IOrderBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(DashboardMetricsResponseDto), Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMetrics(
        [FromServices] IGetDashboardMetricsUseCase useCase)
    {
        var response = await useCase.Execute();
        return Ok(response);
    }
}
