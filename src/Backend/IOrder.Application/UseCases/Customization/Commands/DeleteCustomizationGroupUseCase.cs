using IOrder.Domain.Repositories.Customization;

namespace IOrder.Application.UseCases.Customization.Commands;

public class DeleteCustomizationGroupUseCase : IDeleteCustomizationGroupUseCase
{
    private readonly ICustomizationWriteOnlyRepository _repository;

    public DeleteCustomizationGroupUseCase(ICustomizationWriteOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task Execute(Guid groupId)
    {
        await _repository.DeleteGroup(groupId);
    }
}
