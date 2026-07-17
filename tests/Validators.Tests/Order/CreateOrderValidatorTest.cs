using CommomTestUtilities.Requests.Order;
using IOrder.Application.UseCases.Order.Commands;
using IOrder.Exceptions;
using Shouldly;

namespace Validators.Tests.Order;

public class CreateOrderValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new CreateOrderValidator();
        var request = CreateOrderRequestBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_CustomerNotes_Too_Long()
    {
        var validator = new CreateOrderValidator();
        var request = CreateOrderRequestBuilder.Build();
        request.CustomerNotes = new string('a', 1001);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.CUSTOMIZE_TOO_LONG);
    }

    [Fact]
    public void Error_DeliveryDate_In_Past()
    {
        var validator = new CreateOrderValidator();
        var request = CreateOrderRequestBuilder.Build();
        request.DeliveryDate = DateTime.UtcNow.AddDays(-1);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.ORDER_DELIVERY_DATE_IN_PAST);
    }
}
