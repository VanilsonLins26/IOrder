using CommomTestUtilities.Requests.Order;
using IOrder.Application.UseCases.Order.Commands;
using IOrder.Communication.Enums;
using IOrder.Exceptions;
using Shouldly;

namespace Validators.Tests.Order;

public class UpdateOrderStatusValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new UpdateOrderStatusValidator();
        var request = UpdateOrderStatusRequestBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Invalid_Status()
    {
        var validator = new UpdateOrderStatusValidator();
        var request = UpdateOrderStatusRequestBuilder.Build();
        request.Status = (OrderStatusDto)999;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.ORDER_INVALID_STATUS);
    }
}
