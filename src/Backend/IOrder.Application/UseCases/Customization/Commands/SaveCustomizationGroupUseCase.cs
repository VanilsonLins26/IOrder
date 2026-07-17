using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Repositories.Customization;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Customization.Commands;

public class SaveCustomizationGroupUseCase : ISaveCustomizationGroupUseCase
{
    private readonly ICustomizationWriteOnlyRepository _writeRepository;

    public SaveCustomizationGroupUseCase(ICustomizationWriteOnlyRepository writeRepository)
    {
        _writeRepository = writeRepository;
    }

    public async Task<CustomizationGroupResponseDto> Execute(Guid productId, SaveCustomizationGroupRequestDto request)
    {
        var group = request.Id.HasValue
            ? throw new ErrorOnValidationException(["Edição de grupo não implementada via este endpoint."])
            : new CustomizationGroup();

        group.ProductId = productId;
        group.Name = request.Name;

        if (!Enum.TryParse<CustomizationGroupType>(request.Type, out var type))
            throw new ErrorOnValidationException(["Tipo de grupo inválido."]);

        group.Type = type;
        group.MinSelections = request.MinSelections;
        group.MaxSelections = request.MaxSelections;
        group.Required = request.Required;
        group.Position = request.Position;

        foreach (var optReq in request.Options)
        {
            var option = new CustomizationOption
            {
                Name = optReq.Name,
                PriceModifier = optReq.PriceModifier,
                Position = optReq.Position
            };
            group.AddOption(option);
        }

        await _writeRepository.SaveGroup(group);
        return group.Adapt<CustomizationGroupResponseDto>();
    }
}
