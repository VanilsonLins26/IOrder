using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Payment.Queries;
using Shouldly;

namespace UseCases.Test.Payment;

public class GetPaymentByOrderUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var payment = PaymentBuilder.Build();
        var useCase = CreateUseCase(payment);

        var result = await useCase.Execute(payment.OrderId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(payment.Id);
        result.OrderId.ShouldBe(payment.OrderId);
        result.Amount.ShouldBe(payment.Amount);
    }

    [Fact]
    public async Task Success_Not_Found()
    {
        var useCase = CreateUseCase(null);

        var result = await useCase.Execute(Guid.NewGuid());

        result.ShouldBeNull();
    }

    private static GetPaymentByOrderUseCase CreateUseCase(IOrder.Domain.Entities.Payment? payment)
    {
        var paymentReadOnly = new PaymentReadOnlyRepositoryBuilder()
            .GetByOrderIdAsync(payment)
            .Build();
        var loggedUser = LoggedUserBuilder.Build();

        return new GetPaymentByOrderUseCase(paymentReadOnly, loggedUser);
    }
}
