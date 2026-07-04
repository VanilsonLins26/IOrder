using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Product.Commands;
using IOrder.Application.UseCases.Product.Queries;
using IOrder.Domain.SeedWork.Pagination;
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
        var filter = new ProductSearchCriteria { PageNumber = 1, PageSize = 10 };

        var useCase = CreateUseCase(filter);
        
        var result = await useCase.Execute(filter);

        result.ShouldNotBeNull();
        result.Count().ShouldBe(2);
        result.First().Name.ShouldBe("Pão");

    }
    private static GetProductsPaged CreateUseCase(ProductSearchCriteria filter)
    {
        var readRepository = new ProductReadOnlyRepositoryBuilder();

        readRepository.GetAllPagFiltroPrecoAsync(filter);



        return new GetProductsPaged(readRepository.Build());

    }
}
