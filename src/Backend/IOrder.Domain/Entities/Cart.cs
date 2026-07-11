using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class Cart : IAggregateRoot
{
    public string UserId { get; set; } = string.Empty;
    public string? UserEmail { get; set; }
    public string? UserPhone { get; set; }
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
    public string CouponCode { get; set; } = string.Empty;
    public decimal CartTotal => Items.Sum(item => item.TotalPrice);
    public List<CartItem> Items { get; set; } = [];

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }


    public void UpdateCartItems(IEnumerable<CartItem> cartItems)
    {
        Items.Clear();
        Items.AddRange(cartItems);
    }

    public void AddCartItem(CartItem cartItem)
    {
        var existingItem = Items.FirstOrDefault(i =>
            i.ProductId == cartItem.ProductId &&
            i.Customize == cartItem.Customize &&
            i.SelectedOptions.Select(o => o.OptionId).Order().SequenceEqual(
                cartItem.SelectedOptions.Select(o => o.OptionId).Order()));

        if (existingItem != null)
        {
            existingItem.Quantity += cartItem.Quantity;
        }
        else
        {
            Items.Add(cartItem);
        }
    }

    public bool ChangeCartItemQuantity(int quantity, Guid cartItemId) 
    {
        var cartItem = Items.FirstOrDefault(ci => ci.Id == cartItemId) ;
        if (cartItem is null)
            return false;

        cartItem.Quantity = quantity;

        return true;
    
    }

    public bool UpdateItemPrice(decimal price, Guid cartItemId)
    {
        var cartItem = Items.FirstOrDefault(ci => ci.Id == cartItemId);
        if (cartItem is null)
            return false;

        cartItem.UnitPrice = price;

        return true;
    }

    public bool RemoveCartItem(Guid cartItemId)
    {
        var cartItem = Items.FirstOrDefault(ci => ci.Id == cartItemId);
        if (cartItem != null)
        {
            Items.Remove(cartItem);
            return true;
        }
        return false;
    }
}
