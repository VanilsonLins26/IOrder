using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Customization;
using IOrder.Application.UseCases.Customization.Commands;
using IOrder.Domain.Entities.Enums;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.Customization;

public class SaveCustomizationGroupUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = SaveCustomizationGroupRequestBuilder.Build();
        var repositoryBuilder = new CustomizationWriteOnlyRepositoryBuilder();
        var useCase = new SaveCustomizationGroupUseCase(repositoryBuilder.Build());

        var response = await useCase.Execute(Guid.NewGuid(), request);

        response.ShouldNotBeNull();
        response.Name.ShouldBe(request.Name);
        response.Type.ShouldBe(request.Type);
        response.Options.ShouldNotBeEmpty();
        response.Options.First().Name.ShouldBe(request.Options.First().Name);
    }

    [Fact]
    public async Task Error_Edit_Group_Not_Implemented()
    {
        var request = SaveCustomizationGroupRequestBuilder.Build();
        request.Id = Guid.NewGuid();
        var repositoryBuilder = new CustomizationWriteOnlyRepositoryBuilder();
        var useCase = new SaveCustomizationGroupUseCase(repositoryBuilder.Build());

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(() => useCase.Execute(Guid.NewGuid(), request));
        exception.GetErrorMessages().ShouldContain("Edição de grupo não implementada via este endpoint.");
    }

    [Fact]
    public async Task Error_Invalid_Type()
    {
        var request = SaveCustomizationGroupRequestBuilder.Build();
        request.Type = "InvalidType";
        var repositoryBuilder = new CustomizationWriteOnlyRepositoryBuilder();
        var useCase = new SaveCustomizationGroupUseCase(repositoryBuilder.Build());

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(() => useCase.Execute(Guid.NewGuid(), request));
        exception.GetErrorMessages().ShouldContain("Tipo de grupo inválido.");
    }
}
