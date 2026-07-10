using IOrder.Communication.Request;
using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Customization.Commands;

public interface ISaveCustomizationGroupUseCase
{
    Task<CustomizationGroupResponseDto> Execute(Guid productId, SaveCustomizationGroupRequestDto request);
}
