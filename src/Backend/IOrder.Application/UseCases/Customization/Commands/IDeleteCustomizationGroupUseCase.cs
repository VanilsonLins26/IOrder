namespace IOrder.Application.UseCases.Customization.Commands;

public interface IDeleteCustomizationGroupUseCase
{
    Task Execute(Guid groupId);
}
