using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Events;
using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class Product : EntityBase, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; private set; }

    private readonly List<PromotionPrice> _promotions = new();
    public IReadOnlyCollection<PromotionPrice> Promotions => _promotions.AsReadOnly();
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; } = 0;
    public bool Customizable { get; set; }
    public Guid StoreId { get; set; }
    public Store? Store { get; set; }
    
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public decimal? CurrentPromotionalPrice { get; private set; }
    public void UpdatePrice(decimal newPrice)
    {
        if (Price != newPrice)
        {
            Price = newPrice;
            AddDomainEvent(new PriceChangedEvent(Id, newPrice));
        }
    }

    public void SetCurrentPromotionalPrice(decimal currentPromotionalPrice)
    {
        CurrentPromotionalPrice = currentPromotionalPrice;
    }
    public void RemovePromotionalPrice()
    {
        CurrentPromotionalPrice = null;
    }

}
