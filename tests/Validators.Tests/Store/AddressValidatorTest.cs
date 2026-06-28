using CommomTestUtilities.Requests.Store;
using IOrder.Application.UseCases.Store;
using IOrder.Exceptions;
using Shouldly;

namespace Validators.Tests.Store;

public class AddressValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new AddressValidator();

        var request = AddressRequestBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(true);
    }

    [Fact]
    public void Error_Zip_Code_Empty()
    {
        var validator = new AddressValidator();

        var request = AddressRequestBuilder.Build();
        request.ZipCode = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.ZIP_CODE_EMPTY);
    }

    [Fact]
    public void Error_Zip_Code_Invalid()
    {
        var validator = new AddressValidator();

        var request = AddressRequestBuilder.Build();
        request.ZipCode = request.ZipCode + "0000";

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.ZIP_CODE_INVALID);
    }

    [Fact]
    public void Error_Street_Empty()
    {
        var validator = new AddressValidator();

        var request = AddressRequestBuilder.Build();
        request.Street = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.STREET_EMPTY);
    }

    [Fact]
    public void Error_Street_Invalid()
    {
        var validator = new AddressValidator();

        var request = AddressRequestBuilder.Build();
        request.Street = new string('A', 151);

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.STREET_INVALID);
    }

    [Fact]
    public void Error_Number_Empty()
    {
        var validator = new AddressValidator();
        var request = AddressRequestBuilder.Build();
        request.Number = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.NUMBER_EMPTY);
    }

    [Fact]
    public void Error_Number_Invalid()
    {
        var validator = new AddressValidator();
        var request = AddressRequestBuilder.Build();
        request.Number = new string('1', 11);
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.NUMBER_INVALID);
    }

    [Fact]
    public void Error_Complement_Invalid()
    {
        var validator = new AddressValidator();
        var request = AddressRequestBuilder.Build();
        request.Complement = new string('A', 51);
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.COMPLEMENT_INVALID);
    }

    [Fact]
    public void Error_Neighborhood_Empty()
    {
        var validator = new AddressValidator();
        var request = AddressRequestBuilder.Build();
        request.Neighborhood = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.NEIGHBORHOOD_EMPTY);
    }

    [Fact]
    public void Error_Neighborhood_Invalid()
    {
        var validator = new AddressValidator();
        var request = AddressRequestBuilder.Build();
        request.Neighborhood = new string('A', 101);
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.NEIGHBORHOOD_INVALID);
    }

    [Fact]
    public void Error_City_Empty()
    {
        var validator = new AddressValidator();
        var request = AddressRequestBuilder.Build();
        request.City = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.CITY_EMPTY);
    }

    [Fact]
    public void Error_City_Invalid()
    {
        var validator = new AddressValidator();
        var request = AddressRequestBuilder.Build();
        request.City = new string('A', 101);
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.CITY_INVALID);
    }

    [Fact]
    public void Error_State_Empty()
    {
        var validator = new AddressValidator();
        var request = AddressRequestBuilder.Build();
        request.State = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.STATE_EMPTY);
    }

    [Fact]
    public void Error_State_Invalid()
    {
        var validator = new AddressValidator();
        var request = AddressRequestBuilder.Build();
        request.State = "ABC";
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.STATE_INVALID);
    }
}
