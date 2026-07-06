using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Product.Commands;
using IOrder.Application.UseCases.Product.Queries;
using IOrder.Domain.Repositories.Product;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;

namespace UseCases.Test.Product;

public class GetProductsPagedUseCaseTest
{
    [Fact]
    public async Task Succes()
    {
        var filterDto = new IOrder.Communication.Request.ProductSearchRequestDto { PageNumber = 1, PageSize = 10 }; var filterCriteria = new ProductSearchCriteria(1, 10, null, null, null, null, null, false);

        var useCase = CreateUseCase(filterCriteria);
        
        var result = await useCase.Execute(filterDto);

        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(2);
        result.Items[0].Name.ShouldBe("Pão");

    }
    private static GetProductsPagedUseCase CreateUseCase(ProductSearchCriteria filter)
    {
        var readRepository = new ProductReadOnlyRepositoryBuilder();

        readRepository.GetAllPagFiltroPrecoAsync(filter);



        return new GetProductsPagedUseCase(readRepository.Build());

    }
}
