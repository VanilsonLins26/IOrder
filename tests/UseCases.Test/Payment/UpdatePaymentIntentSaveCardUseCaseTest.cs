using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Payment.Commands;
using Shouldly;
using Xunit;

namespace UseCases.Test.Payment;

public class UpdatePaymentIntentSaveCardUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var order = OrderBuilder.Build();
        var payment = PaymentBuilder.Build(order.Id);

        var paymentReadOnlyBuilder = new PaymentReadOnlyRepositoryBuilder();
        paymentReadOnlyBuilder.BuildGetByOrderIdAsync(payment);

        var paymentServiceBuilder = new PaymentServiceBuilder();
        paymentServiceBuilder.BuildUpdatePaymentIntentSetupFutureUsageAsync();

        var useCase = new UpdatePaymentIntentSaveCardUseCase(
            paymentReadOnlyBuilder.Build(),
            paymentServiceBuilder.Build()
        );

        await Should.NotThrowAsync(() => useCase.Execute(order.Id, true));
    }
}
