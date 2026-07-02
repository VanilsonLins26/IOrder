using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Category;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Category.Queries;

public class GetCategoriesByStoreUseCase : IGetCategoriesByStoreUseCase
{
    private readonly ICategoryReadOnlyRepository _readOnlyRepository;

    public GetCategoriesByStoreUseCase(ICategoryReadOnlyRepository readOnlyRepository)
    {
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<IList<CategoryResponseDto>> Execute(Guid storeId)
    {
        var categories = await _readOnlyRepository.GetAll(storeId);
        
        return categories.OrderBy(c => c.Position)
                         .Adapt<IList<CategoryResponseDto>>();
    }
}

