using Bogus;
using IOrder.Communication.Enums;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests.Order;

public class UpdateOrderStatusRequestBuilder
{
    public static UpdateOrderStatusRequestDto Build()
    {
        return new Faker<UpdateOrderStatusRequestDto>("pt_BR")
            .RuleFor(r => r.Status, OrderStatusDto.AwaitingPayment)
            .Generate();
    }
}
