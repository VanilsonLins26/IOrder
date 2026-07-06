namespace IOrder.Communication.Response;

public class CouponResponseDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public decimal? MinPurchaseAmount { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int MaxUsageCount { get; set; }
    public int CurrentUsageCount { get; set; }
    public bool Active { get; set; }
}
