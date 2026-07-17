using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Delivery;

public interface IDeliveryAssignmentWriteOnlyRepository
{
    Task<DeliveryAssignment> CreateAsync(DeliveryAssignment assignment);
    DeliveryAssignment Update(DeliveryAssignment assignment);
    Task<DeliveryAssignment?> GetByIdTrackingAsync(Guid id);
}
