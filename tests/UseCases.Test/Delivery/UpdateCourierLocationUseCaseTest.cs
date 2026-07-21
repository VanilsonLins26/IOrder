using CommomTestUtilities.Repositories;
using CommomTestUtilities.Repositories.Delivery;
using CommomTestUtilities.Requests;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Delivery.Commands;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System.Linq;

namespace UseCases.Test.Delivery;

public class UpdateCourierLocationUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = UpdateCourierLocationRequestBuilder.Build();
        var useCase = CreateUseCase();

        Func<Task> act = async () => await useCase.Execute(request);

        await act.ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Error_Invalid_Latitude()
    {
        var request = UpdateCourierLocationRequestBuilder.Build();
        request.Latitude = 95; // Invalid latitude

        var useCase = CreateUseCase();

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("Latitude deve estar entre -90 e 90.");
    }

    [Fact]
    public async Task Error_Invalid_Longitude()
    {
        var request = UpdateCourierLocationRequestBuilder.Build();
        request.Longitude = 185; // Invalid longitude

        var useCase = CreateUseCase();

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("Longitude deve estar entre -180 e 180.");
    }

    private UpdateCourierLocationUseCase CreateUseCase()
    {
        var loggedUser = LoggedUserBuilder.Build();
        var repository = new CourierLocationWriteOnlyRepositoryBuilder();
        var uow = UnitOfWorkBuilder.Build();
        var validator = new UpdateCourierLocationValidator();

        return new UpdateCourierLocationUseCase(
            validator,
            loggedUser,
            repository.Build(),
            uow);
    }
}
