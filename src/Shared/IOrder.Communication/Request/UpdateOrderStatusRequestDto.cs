using IOrder.Communication.Enums;

namespace IOrder.Communication.Request;

public class UpdateOrderStatusRequestDto
{
    public OrderStatusDto Status { get; set; }
}
