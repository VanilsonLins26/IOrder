namespace IOrder.Domain.Repositories.Customization;

public interface ICustomizationReadOnlyRepository
{
    Task<List<Entities.CustomizationGroup>> GetByProductId(Guid productId);
}
