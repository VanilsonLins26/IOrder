using CommomTestUtilities.Requests.Store;
using IOrder.Application.UseCases.Store;
using IOrder.Exceptions;
using Shouldly;
using Xunit;

namespace Validators.Tests.Store;

public class UpdateStoreValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new UpdateStoreValidator();
        var request = UpdateStoreRequestBuilder.Build();
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(true);
    }

    [Fact]
    public void Error_Name_Empty()
    {
        var validator = new UpdateStoreValidator();
        var request = UpdateStoreRequestBuilder.Build();
        request.Name = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.NAME_EMPTY);
    }

    [Fact]
    public void Error_About_Empty()
    {
        var validator = new UpdateStoreValidator();
        var request = UpdateStoreRequestBuilder.Build();
        request.About = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.ABOUT_EMPTY);
    }

    [Fact]
    public void Error_ImageUrl_Empty()
    {
        var validator = new UpdateStoreValidator();
        var request = UpdateStoreRequestBuilder.Build();
        request.ImageUrl = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.IMAGE_URL_EMPTY);
    }
}
