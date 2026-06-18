namespace IOrder.Domain.Entities;

public class Product : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public IEnumerable<PromotionPrice>? Promotions { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; } = 0;
    public bool Customizable { get; set; }
    //public int StoreId { get; set; }
    //public Store? Store { get; set; }
    public decimal? CurrentPromotionalPrice { get; private set; }




    public void SetCurrentPromotionalPrice(decimal currentPromotionalPrice)
    {
        CurrentPromotionalPrice = currentPromotionalPrice;
    }
    public void RemovePromotionalPrice()
    {
        CurrentPromotionalPrice = null;
    }

}
