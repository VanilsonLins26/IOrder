using IOrder.Domain.Services;
using IOrder.Communication.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IOrder.API.Controllers;

[Authorize]
public class UploadController : IOrderBaseController
{

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UploadImage(
        [FromServices] IStorageService storageService,
        IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ResponseErrorDto("Nenhuma imagem foi enviada."));
        }

        using var stream = file.OpenReadStream();
        var imageUrl = await storageService.UploadImageAsync(stream, file.FileName);

        return Ok(new { ImageUrl = imageUrl });
    }

}
