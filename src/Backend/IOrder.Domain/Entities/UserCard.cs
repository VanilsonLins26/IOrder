using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class UserCard : EntityBase, IAggregateRoot
{
    public string UserId { get; set; } = string.Empty;
    public string GatewayCardId { get; set; } = string.Empty;
    public string LastFourDigits { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int ExpirationMonth { get; set; }
    public int ExpirationYear { get; set; }
}
