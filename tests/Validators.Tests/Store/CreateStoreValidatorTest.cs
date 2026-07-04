using System.Linq;
using CommomTestUtilities.Requests.Store;
using IOrder.Application.UseCases.Store.Commands;
using IOrder.Exceptions;
using Shouldly;
using Xunit;

namespace Validators.Tests.Store;

public class CreateStoreValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new CreateStoreValidator();
        var request = StoreRequestBuilder.Build();
        
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(true);
    }

    [Fact]
    public void Error_Name_Empty()
    {
        var validator = new CreateStoreValidator();
        var request = StoreRequestBuilder.Build();
        request.Name = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.NAME_EMPTY);
    }

    [Fact]
    public void Error_About_Empty()
    {
        var validator = new CreateStoreValidator();
        var request = StoreRequestBuilder.Build();
        request.About = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.ABOUT_EMPTY);
    }

    [Fact]
    public void Error_ImageUrl_Empty()
    {
        var validator = new CreateStoreValidator();
        var request = StoreRequestBuilder.Build();
        request.ImageUrl = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.IMAGE_URL_EMPTY);
    }

    [Fact]
    public void Error_OpeningHours_Empty()
    {
        var validator = new CreateStoreValidator();
        var request = StoreRequestBuilder.Build();
        request.OpeningHours = [];
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.OPENING_HOURS_EMPTY);
    }

    [Fact]
    public void Error_OpeningHours_Duplicated_Day()
    {
        var validator = new CreateStoreValidator();
        var request = StoreRequestBuilder.Build();
        
        var hour = OpeningHourRequestBuilder.Build();
        hour.DayOfWeek = System.DayOfWeek.Monday;

        var hour2 = OpeningHourRequestBuilder.Build();
        hour2.DayOfWeek = System.DayOfWeek.Monday;

        request.OpeningHours = [hour, hour2];

        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.DUPLICATED_DAY_OF_WEEK);
    }
}
