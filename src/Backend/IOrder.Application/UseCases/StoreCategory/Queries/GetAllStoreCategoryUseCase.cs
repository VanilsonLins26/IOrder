using IOrder.Communication.Response;
using IOrder.Domain.Repositories.StoreCategory;
using Mapster;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.StoreCategory.Queries;

public class GetAllStoreCategoryUseCase : IGetAllStoreCategoryUseCase
{
    private readonly IStoreCategoryReadOnlyRepository _repository;

    public GetAllStoreCategoryUseCase(IStoreCategoryReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<IList<StoreCategoryResponseDto>> Execute()
    {
        var categories = await _repository.GetAllActive();

        return categories.Adapt<IList<StoreCategoryResponseDto>>();
    }
}

