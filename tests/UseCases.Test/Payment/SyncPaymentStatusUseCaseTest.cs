using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Payment.Commands;
using IOrder.Communication.Enums;
using IOrder.Communication.Response;
using Shouldly;
using Xunit;

namespace UseCases.Test.Payment;

public class SyncPaymentStatusUseCaseTest
{
    [Fact]
    public async Task Success_Approved()
    {
        var order = OrderBuilder.Build();
        var payment = PaymentBuilder.Build(order.Id);
        payment.Status = IOrder.Domain.Entities.Enums.PaymentStatus.Pending;

        var paymentResponse = new PaymentResponseDto { Status = PaymentStatusDto.Approved };

        var paymentReadOnlyBuilder = new PaymentReadOnlyRepositoryBuilder();
        paymentReadOnlyBuilder.BuildGetByOrderIdAsync(payment);

        var orderReadOnlyBuilder = new OrderReadOnlyRepositoryBuilder().GetByIdAsync(order);

        var paymentServiceBuilder = new PaymentServiceBuilder();
        paymentServiceBuilder.BuildGetPaymentByStripeId(paymentResponse);

        var paymentWriteOnlyBuilder = new PaymentWriteOnlyRepositoryBuilder();
        var orderWriteOnlyBuilder = new OrderWriteOnlyRepositoryBuilder();
        var unitOfWorkBuilder = UnitOfWorkBuilder.Build();
        var domainEventDispatcherBuilder = new DomainEventDispatcherBuilder();

        var useCase = new SyncPaymentStatusUseCase(
            paymentReadOnlyBuilder.Build(),
            paymentWriteOnlyBuilder.Build(),
            orderReadOnlyBuilder.Build(),
            orderWriteOnlyBuilder.Build(),
            unitOfWorkBuilder,
            domainEventDispatcherBuilder.Build(),
            paymentServiceBuilder.Build()
        );

        await Should.NotThrowAsync(() => useCase.Execute(order.Id));
    }

    [Fact]
    public async Task Success_Rejected()
    {
        var order = OrderBuilder.Build();
        var payment = PaymentBuilder.Build(order.Id);
        payment.Status = IOrder.Domain.Entities.Enums.PaymentStatus.Pending;

        var paymentResponse = new PaymentResponseDto { Status = PaymentStatusDto.Rejected };

        var paymentReadOnlyBuilder = new PaymentReadOnlyRepositoryBuilder();
        paymentReadOnlyBuilder.BuildGetByOrderIdAsync(payment);

        var orderReadOnlyBuilder = new OrderReadOnlyRepositoryBuilder().GetByIdAsync(order);

        var paymentServiceBuilder = new PaymentServiceBuilder();
        paymentServiceBuilder.BuildGetPaymentByStripeId(paymentResponse);

        var paymentWriteOnlyBuilder = new PaymentWriteOnlyRepositoryBuilder();
        var orderWriteOnlyBuilder = new OrderWriteOnlyRepositoryBuilder();
        var unitOfWorkBuilder = UnitOfWorkBuilder.Build();
        var domainEventDispatcherBuilder = new DomainEventDispatcherBuilder();

        var useCase = new SyncPaymentStatusUseCase(
            paymentReadOnlyBuilder.Build(),
            paymentWriteOnlyBuilder.Build(),
            orderReadOnlyBuilder.Build(),
            orderWriteOnlyBuilder.Build(),
            unitOfWorkBuilder,
            domainEventDispatcherBuilder.Build(),
            paymentServiceBuilder.Build()
        );

        await Should.NotThrowAsync(() => useCase.Execute(order.Id));
    }
}
