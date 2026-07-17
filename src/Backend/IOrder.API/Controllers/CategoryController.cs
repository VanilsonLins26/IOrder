using IOrder.Application.UseCases.Category.Commands;
using IOrder.Application.UseCases.Category.Queries;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOrder.API.Controllers;

[Authorize]
public class CategoryController : IOrderBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromServices] ICreateCategoryUseCase useCase,
        [FromBody] CategoryRequestDto request)
    {
        var response = await useCase.Execute(request);
        return Created(string.Empty, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromServices] IUpdateCategoryUseCase useCase,
        [FromRoute] Guid id,
        [FromBody] CategoryRequestDto request)
    {
        var response = await useCase.Execute(id, request);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromServices] IDeleteCategoryUseCase useCase,
        [FromRoute] Guid id)
    {
        var response = await useCase.Execute(id);
        return Ok(response);
    }

    [HttpGet("store/{storeId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IList<CategoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByStore(
        [FromServices] IGetCategoriesByStoreUseCase useCase,
        [FromRoute] Guid storeId)
    {
        var response = await useCase.Execute(storeId);
        return Ok(response);
    }

    [HttpPost("{categoryId:guid}/products")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddProducts(
        [FromServices] IAddProductsToCategoryUseCase useCase,
        [FromRoute] Guid categoryId,
        [FromBody] AddProductsToCategoryRequestDto request)
    {
        var response = await useCase.Execute(categoryId, request);
        return Ok(response);
    }

    [HttpDelete("{categoryId:guid}/products")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmptyProducts(
        [FromServices] IEmptyCategoryUseCase useCase,
        [FromRoute] Guid categoryId)
    {
        var response = await useCase.Execute(categoryId);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromServices] IGetCategoryByIdUseCase useCase,
        [FromRoute] Guid id)
    {
        var response = await useCase.Execute(id);
        return Ok(response);
    }

    [HttpPut("positions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdatePositions(
        [FromServices] IUpdateCategoryPositionsUseCase useCase,
        [FromBody] UpdateCategoryPositionsRequestDto request)
    {
        await useCase.Execute(request);
        return NoContent();
    }
}

