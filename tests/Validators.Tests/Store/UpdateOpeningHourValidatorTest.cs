using System.Linq;
using CommomTestUtilities.Requests.Store;
using IOrder.Application.UseCases.Store;
using IOrder.Exceptions;
using Shouldly;
using Xunit;

namespace Validators.Tests.Store;

public class UpdateOpeningHourValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new UpdateOpeningHourValidator();
        var request = UpdateOpeningHourRequestBuilder.Build();
        
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(true);
    }

    [Fact]
    public void Error_OpeningHours_Empty()
    {
        var validator = new UpdateOpeningHourValidator();
        var request = UpdateOpeningHourRequestBuilder.Build();
        request.OpeningHours = []; 
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.OPENING_HOURS_EMPTY);
    }

    [Fact]
    public void Error_OpeningHours_Duplicated_Day()
    {
        var validator = new UpdateOpeningHourValidator();
        var request = UpdateOpeningHourRequestBuilder.Build();
        
        var hour = OpeningHourRequestBuilder.Build();
        hour.DayOfWeek = 1;

        var hour2 = OpeningHourRequestBuilder.Build();
        hour2.DayOfWeek = 1;

        request.OpeningHours = [hour, hour2];

        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.DUPLICATED_DAY_OF_WEEK);
    }
}
