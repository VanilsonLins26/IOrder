using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Customization;
using Mapster;

namespace IOrder.Application.UseCases.Customization.Queries;

public class GetCustomizationsUseCase : IGetCustomizationsUseCase
{
    private readonly ICustomizationReadOnlyRepository _repository;

    public GetCustomizationsUseCase(ICustomizationReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CustomizationGroupResponseDto>> Execute(Guid productId)
    {
        var groups = await _repository.GetByProductId(productId);
        return groups.Adapt<List<CustomizationGroupResponseDto>>();
    }
}
