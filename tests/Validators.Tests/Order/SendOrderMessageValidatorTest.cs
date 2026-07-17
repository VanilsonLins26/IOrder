using CommomTestUtilities.Requests.Order;
using IOrder.Application.UseCases.Order.Commands;
using IOrder.Communication.Enums;
using IOrder.Exceptions;
using Shouldly;

namespace Validators.Tests.Order;

public class SendOrderMessageValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new SendOrderMessageValidator();
        var request = SendOrderMessageRequestBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Message_Empty()
    {
        var validator = new SendOrderMessageValidator();
        var request = SendOrderMessageRequestBuilder.Build();
        request.Message = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.ORDER_MESSAGE_EMPTY);
    }

    [Fact]
    public void Error_Message_Too_Long()
    {
        var validator = new SendOrderMessageValidator();
        var request = SendOrderMessageRequestBuilder.Build();
        request.Message = new string('a', 1001);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.CUSTOMIZE_TOO_LONG);
    }

    [Fact]
    public void Error_ProposedTotalAmount_Zero_On_Proposal()
    {
        var validator = new SendOrderMessageValidator();
        var request = SendOrderMessageRequestBuilder.Build();
        request.Type = MessageTypeDto.Proposal;
        request.ProposedTotalAmount = 0;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PRICE_GREATER_THAN_0);
    }

    [Fact]
    public void Error_ProposedDeliveryDate_In_Past()
    {
        var validator = new SendOrderMessageValidator();
        var request = SendOrderMessageRequestBuilder.Build();
        request.ProposedDeliveryDate = DateTime.UtcNow.AddDays(-1);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.ORDER_DELIVERY_DATE_IN_PAST);
    }
}
