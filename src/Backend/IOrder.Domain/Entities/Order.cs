using IOrder.Domain.Entities.Enums;
using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class Order : EntityBase, IAggregateRoot
{
    public string UserId { get; set; } = string.Empty;
    public Guid StoreId { get; set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public decimal OriginalAmount { get; set; }
    public string? CouponCode { get; set; }
    public decimal? DiscountValue { get; set; }
    public decimal? DiscountedTotal { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string? CustomerNotes { get; set; }
    public string? ShopkeeperNotes { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private readonly List<OrderMessage> _messages = [];
    public IReadOnlyCollection<OrderMessage> Messages => _messages.AsReadOnly();

    public void AddItem(OrderItem item)
    {
        _items.Add(item);
    }

    public void AddItems(IEnumerable<OrderItem> items)
    {
        _items.AddRange(items);
    }

    public void AddMessage(OrderMessage message)
    {
        _messages.Add(message);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Negotiate(decimal? newTotalAmount, DateTime? newDeliveryDate, string? shopkeeperNotes)
    {
        Status = OrderStatus.Negotiating;
        if (newTotalAmount.HasValue)
            TotalAmount = newTotalAmount.Value;
        if (newDeliveryDate.HasValue)
            DeliveryDate = newDeliveryDate.Value;
        if (shopkeeperNotes is not null)
            ShopkeeperNotes = shopkeeperNotes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Accept()
    {
        Status = OrderStatus.AwaitingPayment;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Decline(string? reason = null)
    {
        Status = OrderStatus.Declined;
        if (reason is not null)
            ShopkeeperNotes = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsPaid()
    {
        Status = OrderStatus.Paid;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsPreparing()
    {
        Status = OrderStatus.Preparing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsReady()
    {
        Status = OrderStatus.Ready;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDelivered()
    {
        Status = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }
}
