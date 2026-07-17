using IOrder.Communication.Enums;

namespace IOrder.Communication.Request;

public class SendOrderMessageRequestDto
{
    public string Message { get; set; } = string.Empty;
    public MessageTypeDto Type { get; set; } = MessageTypeDto.Text;
    public decimal? ProposedTotalAmount { get; set; }
    public DateTime? ProposedDeliveryDate { get; set; }
}
