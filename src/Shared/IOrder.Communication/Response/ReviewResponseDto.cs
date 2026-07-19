namespace IOrder.Communication.Response;

public class ReviewResponseDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid StoreId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? CourierUserId { get; set; }
    public int StoreRating { get; set; }
    public int? CourierRating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
