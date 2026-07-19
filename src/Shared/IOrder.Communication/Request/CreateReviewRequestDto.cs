namespace IOrder.Communication.Request;

public class CreateReviewRequestDto
{
    public int StoreRating { get; set; }
    public int? CourierRating { get; set; }
    public string? Comment { get; set; }
}
