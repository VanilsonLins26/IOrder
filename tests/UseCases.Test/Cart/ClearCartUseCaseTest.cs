using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Cart.Commands;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.Cart;

public class ClearCartUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var loggedUser = LoggedUserBuilder.Build();
        var writeOnlyRepository = new CartWriteOnlyRepositoryBuilder().Build();
        
        var useCase = new ClearCartUseCase(loggedUser, writeOnlyRepository);

        Func<Task> act = async () => await useCase.Execute();

        await act.ShouldNotThrowAsync();
    }
}
