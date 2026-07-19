using IOrder.Communication.Enums;

namespace IOrder.Communication.Response;

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public OrderStatusDto Status { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal OriginalAmount { get; set; }
    public string? CouponCode { get; set; }
    public decimal? DiscountValue { get; set; }
    public decimal? DiscountedTotal { get; set; }
    public DeliveryTypeDto DeliveryType { get; set; }
    public int DeliveryPartner { get; set; }
    public decimal DeliveryFee { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime? DeliveryDateEnd { get; set; }
    public bool RequestedEarlyDelivery { get; set; }
    public bool IsSearchingCourier { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerNotes { get; set; }
    public string? ShopkeeperNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = [];
    public List<OrderMessageResponseDto> Messages { get; set; } = [];
    public DeliveryAssignmentResponseDto? ActiveAssignment { get; set; }
}
