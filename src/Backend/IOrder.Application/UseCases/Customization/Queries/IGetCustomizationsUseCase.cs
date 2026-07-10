using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Customization.Queries;

public interface IGetCustomizationsUseCase
{
    Task<List<CustomizationGroupResponseDto>> Execute(Guid productId);
}
