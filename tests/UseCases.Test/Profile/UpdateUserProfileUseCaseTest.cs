using CommomTestUtilities.Entities;
using CommomTestUtilities.Requests.Profile;
using CommomTestUtilities.Services;
using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Profile.Commands;
using Moq;
using Shouldly;
using Xunit;

namespace UseCases.Test.Profile;

public class UpdateUserProfileUseCaseTest
{
    [Fact]
    public async Task Success_Profile_Exists()
    {
        var profile = UserProfileBuilder.Build();
        var request = UserProfileRequestBuilder.Build();
        
        var useCase = CreateUseCase(
            userId: profile.UserId,
            profile: profile);

        var response = await useCase.Execute(request);

        response.ShouldNotBeNull();
        response.Email.ShouldBe(request.Email);
        response.Phone.ShouldBe(request.Phone);
        
        profile.Email.ShouldBe(request.Email);
        profile.Phone.ShouldBe(request.Phone);
        profile.EmailManuallySet.ShouldBe(true);
    }

    [Fact]
    public async Task Success_Profile_Null_Should_Create()
    {
        var userId = "auth0|test_new";
        var request = UserProfileRequestBuilder.Build();
        
        var writeOnlyBuilder = new ProfileWriteOnlyRepositoryBuilder();

        var useCase = CreateUseCase(
            userId: userId,
            profile: null,
            writeOnlyBuilder: writeOnlyBuilder);

        var response = await useCase.Execute(request);

        response.ShouldNotBeNull();
        response.Email.ShouldBe(request.Email);
        response.Phone.ShouldBe(request.Phone);
    }

    [Fact]
    public async Task Success_Request_Email_Empty_Should_Not_Update_Email()
    {
        var profile = UserProfileBuilder.Build();
        var oldEmail = profile.Email;
        var request = UserProfileRequestBuilder.Build();
        request.Email = string.Empty;
        
        var useCase = CreateUseCase(
            userId: profile.UserId,
            profile: profile);

        var response = await useCase.Execute(request);

        response.ShouldNotBeNull();
        response.Phone.ShouldBe(request.Phone);
        response.Email.ShouldBe(oldEmail);
        
        profile.Phone.ShouldBe(request.Phone);
        profile.Email.ShouldBe(oldEmail);
    }

    private UpdateUserProfileUseCase CreateUseCase(
        string userId,
        IOrder.Domain.Entities.UserProfile? profile,
        ProfileWriteOnlyRepositoryBuilder? writeOnlyBuilder = null)
    {
        var readOnlyRepo = new ProfileReadOnlyRepositoryBuilder().GetByUserId(profile).Build();
        var writeOnlyRepo = writeOnlyBuilder?.Build() ?? new ProfileWriteOnlyRepositoryBuilder().Build();
        var loggedUser = LoggedUserBuilder.Build(userId);
        var uof = UnitOfWorkBuilder.Build();

        return new UpdateUserProfileUseCase(readOnlyRepo, writeOnlyRepo, uof, loggedUser);
    }
}
