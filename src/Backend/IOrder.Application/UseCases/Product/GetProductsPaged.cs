using IOrder.Communication.Response;
using IOrder.Domain.Pagination;
using IOrder.Domain.Repositories.Product;
using IOrder.Domain.SeedWork.Pagination;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Product;

public class GetProductsPaged : IGetProductsPaged
{
    private readonly IProductReadOnlyRepository _readOnlyRepository;

    public GetProductsPaged(IProductReadOnlyRepository readOnlyRepository)
    {
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<IEnumerable<ProductResponseDto>> Execute(ProductSearchQuery query)
    {
        var products = await _readOnlyRepository.GetAllPagFiltroPrecoAsync(query);

        return products.Adapt<PagedList<ProductResponseDto>>();

    }
}
