namespace IOrder.Domain.Entities;

public class EntityBase
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public bool Active { get; set; } = true;
}
