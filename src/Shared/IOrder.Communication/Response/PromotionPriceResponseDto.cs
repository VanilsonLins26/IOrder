namespace IOrder.Communication.Response;

public class PromotionPriceResponseDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public DateTime InitialTime { get; set; }
    public DateTime FinalTime { get; set; }
    public bool Active { get; set; }
}
