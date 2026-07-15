using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Responses.Payment;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.UserCard.Queries;
using Shouldly;
using Xunit;

namespace UseCases.Test.UserCard;

public class GetUserCardsUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var profile = UserProfileBuilder.Build();
        var cards = PaymentCardResponseBuilder.Build(2);

        var paymentBuilder = new PaymentServiceBuilder();
        paymentBuilder.BuildListCards(cards);

        var useCase = CreateUseCase(profile.UserId, profile, paymentBuilder);

        var response = await useCase.Execute();

        response.ShouldNotBeNull();
        response.Count.ShouldBe(cards.Count);
    }

    [Fact]
    public async Task Success_Profile_Null_Should_Return_Empty()
    {
        var useCase = CreateUseCase("userId", null);

        var response = await useCase.Execute();

        response.ShouldNotBeNull();
        response.ShouldBeEmpty();
    }

    [Fact]
    public async Task Success_StripeCustomerId_Empty_Should_Return_Empty()
    {
        var profile = UserProfileBuilder.Build();
        profile.StripeCustomerId = string.Empty;

        var useCase = CreateUseCase(profile.UserId, profile);

        var response = await useCase.Execute();

        response.ShouldNotBeNull();
        response.ShouldBeEmpty();
    }

    private GetUserCardsUseCase CreateUseCase(
        string userId,
        IOrder.Domain.Entities.UserProfile? profile,
        PaymentServiceBuilder? paymentBuilder = null)
    {
        var readOnlyRepo = new ProfileReadOnlyRepositoryBuilder().GetByUserId(profile).Build();
        var paymentService = paymentBuilder?.Build() ?? new PaymentServiceBuilder().Build();
        var loggedUser = LoggedUserBuilder.Build(userId);

        return new GetUserCardsUseCase(readOnlyRepo, paymentService, loggedUser);
    }
}
