using CommomTestUtilities.Requests.Store;
using IOrder.Application.UseCases.Store.Commands;
using IOrder.Exceptions;
using Shouldly;
using Xunit;

namespace Validators.Tests.Store;

public class OpeningHourValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new OpeningHourValidator();
        var request = OpeningHourRequestBuilder.Build();
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(true);
    }

    [Fact]
    public void Error_DayOfWeek_Empty()
    {
        var validator = new OpeningHourValidator();
        var request = OpeningHourRequestBuilder.Build();
        request.DayOfWeek = null;
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.DAY_OF_WEEK_EMPTY);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(7)]
    [InlineData(10)]
    public void Error_DayOfWeek_Invalid(int invalidDay)
    {
        var validator = new OpeningHourValidator();
        var request = OpeningHourRequestBuilder.Build();
        request.DayOfWeek = invalidDay;
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.DAY_OF_WEEK_INVALID);
    }
}
