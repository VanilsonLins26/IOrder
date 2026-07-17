using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.UserCard.Commands;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.UserCard;

public class DeleteUserCardUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var profile = UserProfileBuilder.Build();

        var paymentBuilder = new PaymentServiceBuilder();
        paymentBuilder.BuildDeleteCard();

        var useCase = CreateUseCase(profile.UserId, profile, paymentBuilder);

        await Should.NotThrowAsync(() => useCase.Execute("card_id"));
    }

    [Fact]
    public async Task Error_Profile_Null_Should_Throw()
    {
        var useCase = CreateUseCase("userId", null);

        var ex = await Should.ThrowAsync<NotFoundException>(() => useCase.Execute("card_id"));
        ex.GetErrorMessages().ShouldContain("Perfil não encontrado.");
    }

    [Fact]
    public async Task Error_StripeCustomerId_Empty_Should_Throw()
    {
        var profile = UserProfileBuilder.Build();
        profile.StripeCustomerId = string.Empty;

        var useCase = CreateUseCase(profile.UserId, profile);

        var ex = await Should.ThrowAsync<NotFoundException>(() => useCase.Execute("card_id"));
        ex.GetErrorMessages().ShouldContain("Cliente Stripe não encontrado.");
    }

    private DeleteUserCardUseCase CreateUseCase(
        string userId,
        IOrder.Domain.Entities.UserProfile? profile,
        PaymentServiceBuilder? paymentBuilder = null)
    {
        var readOnlyRepo = new ProfileReadOnlyRepositoryBuilder().GetByUserId(profile).Build();
        var paymentService = paymentBuilder?.Build() ?? new PaymentServiceBuilder().Build();
        var loggedUser = LoggedUserBuilder.Build(userId);

        return new DeleteUserCardUseCase(readOnlyRepo, paymentService, loggedUser);
    }
}
