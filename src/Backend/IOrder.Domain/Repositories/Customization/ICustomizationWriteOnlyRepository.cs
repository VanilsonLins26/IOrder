namespace IOrder.Domain.Repositories.Customization;

public interface ICustomizationWriteOnlyRepository
{
    Task SaveGroup(Entities.CustomizationGroup group);
    Task DeleteGroup(Guid groupId);
    Task DeleteOption(Guid optionId);
}
