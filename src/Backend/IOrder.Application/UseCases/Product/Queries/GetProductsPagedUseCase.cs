using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Repositories.Product;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Product.Queries;

public class GetProductsPagedUseCase : IGetProductsPagedUseCase
{
    private readonly IProductReadOnlyRepository _readOnlyRepository;

    public GetProductsPagedUseCase(IProductReadOnlyRepository readOnlyRepository)
    {
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<PagedResponse<ProductResponseDto>> Execute(ProductSearchRequestDto request)
    {
        var criteria = new ProductSearchCriteria(
            request.PageNumber, 
            request.PageSize, 
            request.Name, 
            request.Price, 
            request.PriceFilter?.Adapt<PriceFilterType>(), 
            request.StoreId, 
            request.OrderBy, 
            request.IsDescending);

        var productsTuple = await _readOnlyRepository.GetAllPagFiltroPrecoAsync(criteria);

        return new PagedResponse<ProductResponseDto>(
            productsTuple.Items.Adapt<List<ProductResponseDto>>(), 
            productsTuple.TotalCount, 
            request.PageNumber, 
            request.PageSize);
    }
}

