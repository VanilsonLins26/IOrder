using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Payment.Commands;
using IOrder.Communication.Enums;
using IOrder.Communication.Response;
using Shouldly;
using Xunit;

namespace UseCases.Test.Payment;

public class ProcessPaymentWebhookUseCaseTest
{
    [Fact]
    public async Task Success_Approved()
    {
        var order = OrderBuilder.Build();
        var payment = PaymentBuilder.Build(order.Id);

        var paymentResponse = new PaymentResponseDto
        {
            Id = payment.Id,
            Status = PaymentStatusDto.Approved
        };

        var paymentServiceBuilder = new PaymentServiceBuilder();
        paymentServiceBuilder.BuildProcessWebhook(paymentResponse);

        var paymentWriteOnlyBuilder = new PaymentWriteOnlyRepositoryBuilder();
        paymentWriteOnlyBuilder.BuildGetByIdTracking(payment);

        var orderWriteOnlyBuilder = new OrderWriteOnlyRepositoryBuilder().GetByIdTracking(order);

        var unitOfWorkBuilder = UnitOfWorkBuilder.Build();
        var domainEventDispatcherBuilder = new DomainEventDispatcherBuilder();

        var useCase = new ProcessPaymentWebhookUseCase(
            paymentServiceBuilder.Build(),
            paymentWriteOnlyBuilder.Build(),
            orderWriteOnlyBuilder.Build(),
            unitOfWorkBuilder,
            domainEventDispatcherBuilder.Build()
        );

        var response = await useCase.Execute("payload", "signature");

        response.ShouldNotBeNull();
        response.Status.ShouldBe(PaymentStatusDto.Approved);
        payment.Status.ShouldBe(IOrder.Domain.Entities.Enums.PaymentStatus.Approved);
        order.Status.ShouldBe(IOrder.Domain.Entities.Enums.OrderStatus.Paid);
    }

    [Fact]
    public async Task Success_Rejected()
    {
        var order = OrderBuilder.Build();
        var payment = PaymentBuilder.Build(order.Id);

        var paymentResponse = new PaymentResponseDto
        {
            Id = payment.Id,
            Status = PaymentStatusDto.Rejected
        };

        var paymentServiceBuilder = new PaymentServiceBuilder();
        paymentServiceBuilder.BuildProcessWebhook(paymentResponse);

        var paymentWriteOnlyBuilder = new PaymentWriteOnlyRepositoryBuilder();
        paymentWriteOnlyBuilder.BuildGetByIdTracking(payment);

        var orderWriteOnlyBuilder = new OrderWriteOnlyRepositoryBuilder().GetByIdTracking(order);

        var unitOfWorkBuilder = UnitOfWorkBuilder.Build();
        var domainEventDispatcherBuilder = new DomainEventDispatcherBuilder();

        var useCase = new ProcessPaymentWebhookUseCase(
            paymentServiceBuilder.Build(),
            paymentWriteOnlyBuilder.Build(),
            orderWriteOnlyBuilder.Build(),
            unitOfWorkBuilder,
            domainEventDispatcherBuilder.Build()
        );

        var response = await useCase.Execute("payload", "signature");

        response.ShouldNotBeNull();
        response.Status.ShouldBe(PaymentStatusDto.Rejected);
        payment.Status.ShouldBe(IOrder.Domain.Entities.Enums.PaymentStatus.Rejected);
    }
}
