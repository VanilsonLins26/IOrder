using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Events;
using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class Order : EntityBase, IAggregateRoot
{
    public string UserId { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public Guid StoreId { get; set; }
    public Store? Store { get; set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public decimal OriginalAmount { get; set; }
    public string? CouponCode { get; set; }
    public decimal? DiscountValue { get; set; }
    public decimal? DiscountedTotal { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DeliveryType DeliveryType { get; set; } = DeliveryType.Delivery;
    public decimal DeliveryFee { get; set; }
    public string? CustomerNotes { get; set; }
    public string? ShopkeeperNotes { get; private set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LastMessageAt { get; private set; }

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
        LastMessageAt = DateTime.UtcNow;
        AddDomainEvent(new NewOrderMessageEvent(Id, message.UserId, message.Message));
    }

    public void Negotiate(decimal? newTotalAmount, DateTime? newDeliveryDate, string? shopkeeperNotes)
    {
        var oldStatus = Status;
        Status = OrderStatus.Negotiating;
        if (shopkeeperNotes is not null)
            ShopkeeperNotes = shopkeeperNotes;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderStatusChangedEvent(Id, UserId, StoreId, oldStatus, Status));
    }

    public void Accept()
    {
        var oldStatus = Status;
        Status = OrderStatus.AwaitingPayment;
        
        var lastProposal = Messages.LastOrDefault(m => m.Type == MessageType.Proposal);
        if (lastProposal != null)
        {
            if (lastProposal.ProposedTotalAmount.HasValue)
                TotalAmount = lastProposal.ProposedTotalAmount.Value;
            if (lastProposal.ProposedDeliveryDate.HasValue)
                DeliveryDate = lastProposal.ProposedDeliveryDate.Value;
        }

        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderStatusChangedEvent(Id, UserId, StoreId, oldStatus, Status));
    }

    public void Decline(string? reason = null)
    {
        var oldStatus = Status;
        Status = OrderStatus.Declined;
        if (reason is not null)
            ShopkeeperNotes = reason;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderStatusChangedEvent(Id, UserId, StoreId, oldStatus, Status));
    }

    public void Cancel()
    {
        var oldStatus = Status;
        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderStatusChangedEvent(Id, UserId, StoreId, oldStatus, Status));
    }

    public void MarkAsPaid()
    {
        var oldStatus = Status;
        Status = OrderStatus.Paid;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderStatusChangedEvent(Id, UserId, StoreId, oldStatus, Status));
    }

    public void MarkAsPreparing()
    {
        var oldStatus = Status;
        Status = OrderStatus.Preparing;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderStatusChangedEvent(Id, UserId, StoreId, oldStatus, Status));
    }

    public void MarkAsReady()
    {
        var oldStatus = Status;
        Status = OrderStatus.Ready;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderStatusChangedEvent(Id, UserId, StoreId, oldStatus, Status));
    }

    public void MarkAsDelivered()
    {
        var oldStatus = Status;
        Status = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderStatusChangedEvent(Id, UserId, StoreId, oldStatus, Status));
    }
}
