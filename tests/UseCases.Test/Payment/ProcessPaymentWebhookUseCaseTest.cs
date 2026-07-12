using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Payment.Commands;
using IOrder.Communication.Enums;
using IOrder.Communication.Response;
using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Events;
using IOrder.Domain.Repositories;
using IOrder.Domain.SeedWork;
using IOrder.Domain.Services;
using Moq;
using Shouldly;

namespace UseCases.Test.Payment;

public class ProcessPaymentWebhookUseCaseTest
{
    [Fact]
    public async Task Success_Approved()
    {
        var payment = PaymentBuilder.Build();
        var order = OrderBuilder.Build(storeId: payment.OrderId);
        var webhookResponse = BuildWebhookResponse(payment, PaymentStatusDto.Approved);
        var useCase = CreateUseCase(webhookResponse, payment, order, out var eventDispatcherMock);

        var result = await useCase.Execute("{}", "signature");

        result.ShouldNotBeNull();
        result.Status.ShouldBe(PaymentStatusDto.Approved);
        payment.Status.ShouldBe(PaymentStatus.Approved);
        payment.PaidAt.ShouldNotBeNull();
        order.Status.ShouldBe(OrderStatus.Paid);
        eventDispatcherMock.Verify(e => e.DispatchAsync(
            It.Is<IList<IDomainEvent>>(events => events.Any(ev => ev is PaymentApprovedEvent))),
            Times.Once);
    }

    [Fact]
    public async Task Success_Rejected()
    {
        var payment = PaymentBuilder.Build();
        var order = OrderBuilder.Build(storeId: payment.OrderId);
        var webhookResponse = BuildWebhookResponse(payment, PaymentStatusDto.Rejected);
        var useCase = CreateUseCase(webhookResponse, payment, order, out var eventDispatcherMock);

        var result = await useCase.Execute("{}", "signature");

        result.ShouldNotBeNull();
        result.Status.ShouldBe(PaymentStatusDto.Rejected);
        payment.Status.ShouldBe(PaymentStatus.Rejected);
        order.Status.ShouldNotBe(OrderStatus.Paid);
        eventDispatcherMock.Verify(e => e.DispatchAsync(
            It.Is<IList<IDomainEvent>>(events => events.Any(ev => ev is PaymentRejectedEvent))),
            Times.Once);
    }

    [Fact]
    public async Task Success_Null_Response()
    {
        var useCase = CreateUseCase(null, null, null, out _);

        var result = await useCase.Execute("{}", "signature");

        result.ShouldBeNull();
    }

    [Fact]
    public async Task Success_Payment_Not_Found()
    {
        var webhookResponse = BuildWebhookResponse(PaymentBuilder.Build(), PaymentStatusDto.Approved);
        var useCase = CreateUseCase(webhookResponse, null, null, out _);

        var result = await useCase.Execute("{}", "signature");

        result.ShouldNotBeNull();
        result.Status.ShouldBe(PaymentStatusDto.Approved);
    }

    [Fact]
    public async Task Success_Order_Not_Found()
    {
        var payment = PaymentBuilder.Build();
        var webhookResponse = BuildWebhookResponse(payment, PaymentStatusDto.Approved);
        var useCase = CreateUseCase(webhookResponse, payment, null, out _);

        var result = await useCase.Execute("{}", "signature");

        result.ShouldNotBeNull();
        result.Status.ShouldBe(PaymentStatusDto.Approved);
    }

    private static ProcessPaymentWebhookUseCase CreateUseCase(
        PaymentResponseDto? webhookResponse,
        IOrder.Domain.Entities.Payment? payment,
        IOrder.Domain.Entities.Order? order,
        out Mock<IDomainEventDispatcher> eventDispatcherMock)
    {
        var paymentService = new PaymentServiceBuilder()
            .ProcessWebhookAsync(webhookResponse)
            .Build();

        var paymentWriteOnly = new PaymentWriteOnlyRepositoryBuilder()
            .GetById(payment)
            .Update()
            .Build();

        var orderWriteOnly = new OrderWriteOnlyRepositoryBuilder();
        if (order is not null)
            orderWriteOnly.GetByIdTracking(order);

        var uow = UnitOfWorkBuilder.Build();

        eventDispatcherMock = new Mock<IDomainEventDispatcher>();
        eventDispatcherMock
            .Setup(e => e.DispatchAsync(It.IsAny<IList<IDomainEvent>>()))
            .Returns(Task.CompletedTask);

        return new ProcessPaymentWebhookUseCase(
            paymentService,
            paymentWriteOnly,
            orderWriteOnly.Build(),
            uow,
            eventDispatcherMock.Object);
    }

    private static PaymentResponseDto BuildWebhookResponse(
        IOrder.Domain.Entities.Payment payment,
        PaymentStatusDto status)
    {
        return new PaymentResponseDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Method = (PaymentMethodDto)payment.Method,
            Status = status,
            CreatedAt = payment.CreatedAt,
            PaidAt = status == PaymentStatusDto.Approved ? DateTime.UtcNow : null,
        };
    }
}
