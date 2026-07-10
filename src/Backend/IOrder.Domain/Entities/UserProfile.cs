using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class UserProfile : EntityBase, IAggregateRoot
{
    public string UserId { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
