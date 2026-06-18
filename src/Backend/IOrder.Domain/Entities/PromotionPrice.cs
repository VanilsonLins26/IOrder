namespace IOrder.Domain.Entities;

public class PromotionPrice : EntityBase
{
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public DateTime InitialTime { get; set; }
    public DateTime FinalTime { get; set; }
    public Product Product { get; set; }

    public bool IsActive()
    {
        var now = DateTime.UtcNow;
        bool isInsideTimeWindow = now >= InitialTime && now <= FinalTime;

       
        return Active && isInsideTimeWindow;
    }
}
