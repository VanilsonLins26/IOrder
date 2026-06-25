using IOrder.Application.UseCases.Product;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Pagination;
using IOrder.Domain.SeedWork.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IOrder.API.Controllers;

public class ProductController : IOrderBaseController
{

    [HttpPost]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromServices] ICreateProductUseCase useCase, [FromBody] ProductRequestDto productRequest)
    {
        var response = await useCase.Execute(productRequest);

        return Created(string.Empty, response);    
    }

    [HttpDelete("{productId:guid}")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromServices] IDeleteProductUseCase useCase, Guid productId)
    {
        var deletedProduct = await useCase.Execute(productId);

        return Ok(deletedProduct);
    }

    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedList<ProductResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromServices] IGetProductsPaged useCase, [FromQuery] ProductSearchQuery query)
    {
        var products = await useCase.Execute(query);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById([FromServices] IGetProductById usecase, Guid id)
    {
        var product = await usecase.Execute(id);

        return Ok(product);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Update([FromServices] IUpdateProductUseCase usecase, Guid id, [FromBody] UpdateProductRequestDto dto)
    {
        var product = await usecase.Execute(id, dto);

        return Ok(product);
    }

    [HttpPost("promotion")]
    [ProducesResponseType(typeof(PromotionPriceResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(PromotionPriceResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(PromotionPriceResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreatePromotion([FromServices] ICreatePromotionPriceUseCase useCase, [FromBody] PromotionPriceResquestDto dto)
    {
        var promotionPrice = await useCase.Execute(dto);

        return Created(string.Empty, promotionPrice);
    }


}
