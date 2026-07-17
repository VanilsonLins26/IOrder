using CommomTestUtilities.Requests.Category;
using IOrder.Application.UseCases.Category.Commands;
using IOrder.Exceptions;
using Shouldly;
using Xunit;

namespace Validators.Tests.Category;

public class UpdateCategoryValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new UpdateCategoryValidator();
        var request = CategoryRequestBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Name_Empty()
    {
        var validator = new UpdateCategoryValidator();
        var request = CategoryRequestBuilder.Build();
        request.Name = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.NAME_EMPTY);
    }

    [Fact]
    public void Error_Position_Null()
    {
        var validator = new UpdateCategoryValidator();
        var request = CategoryRequestBuilder.Build();
        request.Position = null;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.POSITION_NULL);
    }

    [Fact]
    public void Error_Invalid_Position()
    {
        var validator = new UpdateCategoryValidator();
        var request = CategoryRequestBuilder.Build();
        request.Position = -1;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.INVALID_POSITION);
    }
}
