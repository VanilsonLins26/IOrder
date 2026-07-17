using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Payment.Queries;
using Shouldly;
using Xunit;

namespace UseCases.Test.Payment;

public class GetPaymentByOrderUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var payment = PaymentBuilder.Build(Guid.NewGuid());
        
        var readOnlyBuilder = new PaymentReadOnlyRepositoryBuilder();
        readOnlyBuilder.BuildGetByOrderIdAsync(payment);
        
        var loggedUserBuilder = LoggedUserBuilder.Build("user123");

        var useCase = new GetPaymentByOrderUseCase(readOnlyBuilder.Build(), loggedUserBuilder);

        var response = await useCase.Execute(payment.OrderId);

        response.ShouldNotBeNull();
        response.Id.ShouldBe(payment.Id);
        response.Status.ToString().ShouldBe(payment.Status.ToString());
    }

    [Fact]
    public async Task Success_Payment_Not_Found_Should_Return_Null()
    {
        var readOnlyBuilder = new PaymentReadOnlyRepositoryBuilder();
        readOnlyBuilder.BuildGetByOrderIdAsync(null);
        
        var loggedUserBuilder = LoggedUserBuilder.Build("user123");

        var useCase = new GetPaymentByOrderUseCase(readOnlyBuilder.Build(), loggedUserBuilder);

        var response = await useCase.Execute(Guid.NewGuid());

        response.ShouldBeNull();
    }
}
