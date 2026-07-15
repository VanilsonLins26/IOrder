using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Payment;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Payment.Commands;
using IOrder.Communication.Response;
using IOrder.Domain.Entities.Enums;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.Payment;

public class CreatePaymentUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var loggedUserBuilder = LoggedUserBuilder.Build("user123");
        var validator = new CreatePaymentValidator();

        var order = OrderBuilder.Build(null, "user123");
        order.Accept(); // Sets status to AwaitingPayment
        var request = CreatePaymentRequestBuilder.Build(order.Id);

        var profile = UserProfileBuilder.Build("user123");
        profile.StripeCustomerId = null; // Forces GetOrCreateCustomerAsync

        var orderReadOnlyBuilder = new OrderReadOnlyRepositoryBuilder().GetByIdAsync(order);
        var orderWriteOnlyBuilder = new OrderWriteOnlyRepositoryBuilder();
        var paymentReadOnlyBuilder = new PaymentReadOnlyRepositoryBuilder();
        var profileReadOnlyBuilder = new ProfileReadOnlyRepositoryBuilder().GetByUserId(profile);
        var profileWriteOnlyBuilder = new ProfileWriteOnlyRepositoryBuilder().GetByUserIdTracking(profile);
        var unitOfWorkBuilder = UnitOfWorkBuilder.Build();

        var paymentResponse = new PaymentIntentResponseDto
        {
            ClientSecret = "secret",
            PaymentIntentId = "intent_123"
        };

        var paymentServiceBuilder = new PaymentServiceBuilder();
        paymentServiceBuilder.BuildGetOrCreateCustomerAsync("cus_123");
        paymentServiceBuilder.BuildCreatePaymentIntent(paymentResponse);

        var useCase = new CreatePaymentUseCase(
            validator,
            loggedUserBuilder,
            orderReadOnlyBuilder.Build(),
            orderWriteOnlyBuilder.Build(),
            paymentReadOnlyBuilder.Build(),
            profileReadOnlyBuilder.Build(),
            profileWriteOnlyBuilder.Build(),
            paymentServiceBuilder.Build(),
            unitOfWorkBuilder
        );

        var response = await useCase.Execute(request);

        response.ShouldNotBeNull();
        response.ClientSecret.ShouldBe("secret");
    }

    [Fact]
    public async Task Error_Order_Not_Found()
    {
        var request = CreatePaymentRequestBuilder.Build(Guid.NewGuid());
        var loggedUserBuilder = LoggedUserBuilder.Build("user123");
        var validator = new CreatePaymentValidator();

        var orderReadOnlyBuilder = new OrderReadOnlyRepositoryBuilder().GetByIdAsync(null);

        var useCase = new CreatePaymentUseCase(
            validator,
            loggedUserBuilder,
            orderReadOnlyBuilder.Build(),
            new OrderWriteOnlyRepositoryBuilder().Build(),
            new PaymentReadOnlyRepositoryBuilder().Build(),
            new ProfileReadOnlyRepositoryBuilder().Build(),
            new ProfileWriteOnlyRepositoryBuilder().Build(),
            new PaymentServiceBuilder().Build(),
            UnitOfWorkBuilder.Build()
        );

        var exception = await Should.ThrowAsync<NotFoundException>(() => useCase.Execute(request));
        exception.GetErrorMessages().ShouldContain(ResourceMessagesException.ORDER_NOT_FOUND);
    }
}
