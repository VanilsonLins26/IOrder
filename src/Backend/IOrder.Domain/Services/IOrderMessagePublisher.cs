namespace IOrder.Domain.Services;

public interface IOrderMessagePublisher
{
    Task PublishMessageAsync(Guid orderId, object message);
}
