using CommomTestUtilities.Entities;
using CommomTestUtilities.Services;
using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Profile.Queries;
using Moq;
using Shouldly;
using Xunit;

namespace UseCases.Test.Profile;

public class GetUserProfileUseCaseTest
{
    [Fact]
    public async Task Success_Profile_Exists()
    {
        var profile = UserProfileBuilder.Build();
        
        var useCase = CreateUseCase(
            userId: profile.UserId,
            jwtEmail: profile.Email,
            profile: profile);

        var response = await useCase.Execute();

        response.ShouldNotBeNull();
        response.Email.ShouldBe(profile.Email);
        response.Phone.ShouldBe(profile.Phone);
    }

    [Fact]
    public async Task Success_Profile_Null_Should_Create()
    {
        var userId = "auth0|test_new";
        var jwtEmail = "test@test.com";
        
        var writeOnlyBuilder = new ProfileWriteOnlyRepositoryBuilder();

        var useCase = CreateUseCase(
            userId: userId,
            jwtEmail: jwtEmail,
            profile: null,
            writeOnlyBuilder: writeOnlyBuilder);

        var response = await useCase.Execute();

        response.ShouldNotBeNull();
        response.Email.ShouldBe(jwtEmail);
        response.Phone.ShouldBe(string.Empty);
    }

    [Fact]
    public async Task Success_Email_Changed_In_Jwt_Should_Update()
    {
        var profile = UserProfileBuilder.Build();
        profile.EmailManuallySet = false;
        profile.Email = "old@test.com";
        
        var newJwtEmail = "new@test.com";

        var writeOnlyBuilder = new ProfileWriteOnlyRepositoryBuilder();

        var useCase = CreateUseCase(
            userId: profile.UserId,
            jwtEmail: newJwtEmail,
            profile: profile,
            writeOnlyBuilder: writeOnlyBuilder);

        var response = await useCase.Execute();

        response.ShouldNotBeNull();
        response.Email.ShouldBe(newJwtEmail);
        response.Phone.ShouldBe(profile.Phone);
        profile.Email.ShouldBe(newJwtEmail);
    }
    
    [Fact]
    public async Task Success_Email_Changed_In_Jwt_But_Manually_Set_Should_Not_Update()
    {
        var profile = UserProfileBuilder.Build();
        profile.EmailManuallySet = true;
        profile.Email = "old@test.com";
        
        var newJwtEmail = "new@test.com";

        var writeOnlyBuilder = new ProfileWriteOnlyRepositoryBuilder();

        var useCase = CreateUseCase(
            userId: profile.UserId,
            jwtEmail: newJwtEmail,
            profile: profile,
            writeOnlyBuilder: writeOnlyBuilder);

        var response = await useCase.Execute();

        response.ShouldNotBeNull();
        response.Email.ShouldBe("old@test.com"); // Should remain old
    }

    private GetUserProfileUseCase CreateUseCase(
        string userId,
        string jwtEmail,
        IOrder.Domain.Entities.UserProfile? profile,
        ProfileWriteOnlyRepositoryBuilder? writeOnlyBuilder = null)
    {
        var readOnlyRepo = new ProfileReadOnlyRepositoryBuilder().GetByUserId(profile).Build();
        var writeOnlyRepo = writeOnlyBuilder?.Build() ?? new ProfileWriteOnlyRepositoryBuilder().Build();
        var loggedUser = LoggedUserBuilder.Build(userId, jwtEmail);
        var uof = UnitOfWorkBuilder.Build();

        return new GetUserProfileUseCase(readOnlyRepo, writeOnlyRepo, loggedUser, uof);
    }
}
