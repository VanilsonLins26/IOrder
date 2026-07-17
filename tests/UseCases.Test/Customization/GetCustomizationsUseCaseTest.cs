using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Customization.Queries;
using Shouldly;
using Xunit;

namespace UseCases.Test.Customization;

public class GetCustomizationsUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var repositoryBuilder = new CustomizationReadOnlyRepositoryBuilder();
        var useCase = new GetCustomizationsUseCase(repositoryBuilder.Build());

        var response = await useCase.Execute(Guid.NewGuid());

        response.ShouldNotBeNull();
        response.ShouldBeEmpty();
    }
}
