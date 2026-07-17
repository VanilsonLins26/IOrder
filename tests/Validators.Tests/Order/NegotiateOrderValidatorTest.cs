using CommomTestUtilities.Requests.Order;
using IOrder.Application.UseCases.Order.Commands;
using IOrder.Exceptions;
using Shouldly;

namespace Validators.Tests.Order;

public class NegotiateOrderValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new NegotiateOrderValidator();
        var request = NegotiateOrderRequestBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_ShopkeeperNotes_Empty()
    {
        var validator = new NegotiateOrderValidator();
        var request = NegotiateOrderRequestBuilder.Build();
        request.ShopkeeperNotes = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.ORDER_MESSAGE_EMPTY);
    }

    [Fact]
    public void Error_ShopkeeperNotes_Too_Long()
    {
        var validator = new NegotiateOrderValidator();
        var request = NegotiateOrderRequestBuilder.Build();
        request.ShopkeeperNotes = new string('a', 1001);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.CUSTOMIZE_TOO_LONG);
    }

    [Fact]
    public void Error_ProposedTotalAmount_Null()
    {
        var validator = new NegotiateOrderValidator();
        var request = NegotiateOrderRequestBuilder.Build();
        request.ProposedTotalAmount = null;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PRICE_EMPTY);
    }

    [Fact]
    public void Error_ProposedTotalAmount_Zero()
    {
        var validator = new NegotiateOrderValidator();
        var request = NegotiateOrderRequestBuilder.Build();
        request.ProposedTotalAmount = 0;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PRICE_GREATER_THAN_0);
    }

    [Fact]
    public void Error_ProposedDeliveryDate_In_Past()
    {
        var validator = new NegotiateOrderValidator();
        var request = NegotiateOrderRequestBuilder.Build();
        request.ProposedDeliveryDate = DateTime.UtcNow.AddDays(-1);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.ORDER_DELIVERY_DATE_IN_PAST);
    }
}
