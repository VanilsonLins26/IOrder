using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Customization.Commands;
using Shouldly;
using Xunit;

namespace UseCases.Test.Customization;

public class DeleteCustomizationGroupUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var repositoryBuilder = new CustomizationWriteOnlyRepositoryBuilder();
        var useCase = new DeleteCustomizationGroupUseCase(repositoryBuilder.Build());

        await Should.NotThrowAsync(() => useCase.Execute(Guid.NewGuid()));
    }
}
