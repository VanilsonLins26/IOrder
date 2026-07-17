namespace IOrder.Domain.Entities.Enums;

public enum OrderStatus
{
    Pending = 0,
    Negotiating = 1,
    AwaitingPayment = 2,
    Paid = 3,
    Preparing = 4,
    Ready = 5,
    Delivered = 6,
    Cancelled = 7,
    Declined = 8,
    OutForDelivery = 9
}
