using CommomTestUtilities.Requests;
using IOrder.Application.UseCases.Delivery.Commands;
using Shouldly;

namespace Validators.Tests.Delivery;

public class UpdateCourierLocationValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new UpdateCourierLocationValidator();
        var request = UpdateCourierLocationRequestBuilder.Build();
        
        var result = validator.Validate(request);
        
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void Error_Latitude_Invalid(double latitude)
    {
        var validator = new UpdateCourierLocationValidator();
        var request = UpdateCourierLocationRequestBuilder.Build();
        request.Latitude = latitude;
        
        var result = validator.Validate(request);
        
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == "Latitude deve estar entre -90 e 90.");
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void Error_Longitude_Invalid(double longitude)
    {
        var validator = new UpdateCourierLocationValidator();
        var request = UpdateCourierLocationRequestBuilder.Build();
        request.Longitude = longitude;
        
        var result = validator.Validate(request);
        
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == "Longitude deve estar entre -180 e 180.");
    }
}
