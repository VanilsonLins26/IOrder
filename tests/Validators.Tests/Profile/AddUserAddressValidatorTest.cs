using CommomTestUtilities.Requests.Profile;
using Shouldly;
using IOrder.Application.UseCases.Profile.Commands;

namespace Validators.Tests.Profile;

public class AddUserAddressValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new AddUserAddressValidator();
        var request = AddUserAddressRequestBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Error_Name_Empty(string name)
    {
        var validator = new AddUserAddressValidator();
        var request = AddUserAddressRequestBuilder.Build();
        request.Name = name;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == "Name is required."); result.Errors.Count.ShouldBe(1);
    }
}
