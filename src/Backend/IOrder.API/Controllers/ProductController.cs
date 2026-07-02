using IOrder.Application.UseCases.Product.Commands;
using IOrder.Application.UseCases.Product.Queries;
using IOrder.Communication.Request;
using IOrder.Communication.Response;

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

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);    
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
    [ProducesResponseType(typeof(PagedResponse<ProductResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromServices] IGetProductsPagedUseCase useCase, [FromQuery] ProductSearchRequestDto request)
    {
        var products = await useCase.Execute(request);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById([FromServices] IGetProductByIdUseCase usecase, Guid id)
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

        return CreatedAtAction(nameof(GetById), new { id = promotionPrice.ProductId }, promotionPrice);
    }


}

