namespace IOrder.Communication.Response;

public class PromotionPriceResponse
{

    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public DateTime InitialTime { get; set; }
    public DateTime FinalTime { get; set; }
    public  bool Active { get; set; }
}
