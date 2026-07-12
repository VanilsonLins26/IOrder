using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Payment;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Payment.Commands;
using IOrder.Communication.Enums;
using IOrder.Communication.Response;
using IOrder.Domain.Entities.Enums;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.Payment;

public class CreatePaymentUseCaseTest
{
    [Fact]
    public async Task Success_PIX()
    {
        var order = OrderBuilder.Build();
        order.Status = OrderStatus.AwaitingPayment;
        var request = CreatePaymentRequestBuilder.BuildPix();
        var expectedResponse = BuildPaymentResponse(PaymentMethodDto.Pix);
        var useCase = CreateUseCase(order, request, expectedResponse);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Method.ShouldBe(PaymentMethodDto.Pix);
        result.Status.ShouldBe(PaymentStatusDto.Pending);
    }

    [Fact]
    public async Task Success_CreditCard()
    {
        var order = OrderBuilder.Build();
        order.Status = OrderStatus.AwaitingPayment;
        var request = CreatePaymentRequestBuilder.BuildCreditCard();
        var expectedResponse = BuildPaymentResponse(PaymentMethodDto.CreditCard);
        var useCase = CreateUseCase(order, request, expectedResponse);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Method.ShouldBe(PaymentMethodDto.CreditCard);
        result.Status.ShouldBe(PaymentStatusDto.Pending);
    }

    [Fact]
    public async Task Success_Boleto()
    {
        var order = OrderBuilder.Build();
        order.Status = OrderStatus.AwaitingPayment;
        var request = CreatePaymentRequestBuilder.BuildBoleto();
        var expectedResponse = BuildPaymentResponse(PaymentMethodDto.Boleto);
        var useCase = CreateUseCase(order, request, expectedResponse);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Method.ShouldBe(PaymentMethodDto.Boleto);
        result.Status.ShouldBe(PaymentStatusDto.Pending);
    }

    [Fact]
    public async Task Error_Order_Not_Found()
    {
        var request = CreatePaymentRequestBuilder.Build();
        var useCase = CreateUseCase(null, request, null);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.ORDER_NOT_FOUND);
    }

    [Fact]
    public async Task Error_Order_Not_Owned()
    {
        var order = OrderBuilder.Build(userId: "other-user");
        order.Status = OrderStatus.AwaitingPayment;
        var request = CreatePaymentRequestBuilder.Build();
        var useCase = CreateUseCase(order, request, null, loggedUserId: "current-user");

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<UnauthorizedStoreException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.ORDER_NOT_FOUND);
    }

    [Fact]
    public async Task Error_Order_Not_Awaiting_Payment()
    {
        var order = OrderBuilder.Build();
        order.Status = OrderStatus.Pending;
        var request = CreatePaymentRequestBuilder.Build();
        var useCase = CreateUseCase(order, request, null);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.PAYMENT_ORDER_NOT_AWAITING);
    }

    private static CreatePaymentUseCase CreateUseCase(
        IOrder.Domain.Entities.Order? order,
        Communication.Request.CreatePaymentRequestDto? request,
        PaymentResponseDto? paymentResponse,
        string loggedUserId = "test-user-id")
    {
        var orderReadOnly = new OrderReadOnlyRepositoryBuilder()
            .GetByIdAsync(order)
            .Build();
        var loggedUser = LoggedUserBuilder.Build(loggedUserId);

        PaymentServiceBuilder paymentServiceBuilder = new();
        if (request is not null && paymentResponse is not null)
        {
            switch (request.Method)
            {
                case PaymentMethodDto.Pix:
                    paymentServiceBuilder.CreatePixPaymentAsync(paymentResponse);
                    break;
                case PaymentMethodDto.CreditCard:
                    paymentServiceBuilder.CreateCardPaymentAsync(paymentResponse);
                    break;
                case PaymentMethodDto.Boleto:
                    paymentServiceBuilder.CreateBoletoPaymentAsync(paymentResponse);
                    break;
            }
        }

        return new CreatePaymentUseCase(
            loggedUser,
            orderReadOnly,
            paymentServiceBuilder.Build());
    }

    private static PaymentResponseDto BuildPaymentResponse(PaymentMethodDto method)
    {
        var faker = new Bogus.Faker("pt_BR");
        return new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            Amount = faker.Finance.Amount(10, 500),
            Method = method,
            Status = PaymentStatusDto.Pending,
            CreatedAt = DateTime.UtcNow,
            PixQrCode = method == PaymentMethodDto.Pix ? faker.Image.PicsumUrl() : null,
            PixCopyPaste = method == PaymentMethodDto.Pix ? faker.Random.AlphaNumeric(60) : null,
            CardLastFourDigits = method == PaymentMethodDto.CreditCard ? faker.Finance.LastFourDigits() : null,
            Installments = method == PaymentMethodDto.CreditCard ? faker.Random.Int(1, 12) : null,
            BoletoUrl = method == PaymentMethodDto.Boleto ? faker.Internet.Url() : null,
            BoletoBarcode = method == PaymentMethodDto.Boleto ? faker.Random.AlphaNumeric(48) : null,
        };
    }
}
