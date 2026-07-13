using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using IOrder.Application.Services.Payment;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Payment;
using IOrder.infrastructure.DataAccess;
using Mapster;
using MercadoPago.Client.Common;
using MercadoPago.Client.Customer;
using MercadoPago.Client.Payment;
using MercadoPago.Error;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IOrder.infrastructure.Services.Payment;

[ExcludeFromCodeCoverage]
public class MercadoPagoService : IPaymentService
{
    private readonly IPaymentWriteOnlyRepository _paymentWriteRepo;
    private readonly IPaymentReadOnlyRepository _paymentReadRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MercadoPagoService> _logger;
    private readonly MercadoPagoSettings _settings;
    private readonly PaymentClient _paymentClient;
    private readonly CustomerClient _customerClient;
    private readonly CustomerCardClient _customerCardClient;

    public MercadoPagoService(
        IPaymentWriteOnlyRepository paymentWriteRepo,
        IPaymentReadOnlyRepository paymentReadRepo,
        IUnitOfWork unitOfWork,
        IOptions<MercadoPagoSettings> settings,
        ILogger<MercadoPagoService> logger)
    {
        _paymentWriteRepo = paymentWriteRepo;
        _paymentReadRepo = paymentReadRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _settings = settings.Value;

        MercadoPago.Config.MercadoPagoConfig.AccessToken = _settings.AccessToken;
        _paymentClient = new PaymentClient();
        _customerClient = new CustomerClient();
        _customerCardClient = new CustomerCardClient();
    }

    public async Task<PaymentResponseDto> CreatePixPaymentAsync(
        Guid orderId, decimal amount, string payerEmail, string? payerIdentification)
    {
        var request = new PaymentCreateRequest
        {
            TransactionAmount = amount,
            PaymentMethodId = "pix",
            Description = $"Pedido #{orderId.ToString("N")[..8].ToUpper()}",
            Payer = new PaymentPayerRequest
            {
                Email = payerEmail,
                FirstName = "APRO"
            },
            NotificationUrl = _settings.WebhookUrl
        };

        _logger.LogInformation(
            "Creating PIX payment: amount={Amount}, payer={Email}",
            amount, payerEmail);

        MercadoPago.Resource.Payment.Payment mpPayment;

        var requestOptions = new MercadoPago.Client.RequestOptions();
        requestOptions.CustomHeaders["X-Idempotency-Key"] = Guid.NewGuid().ToString("N");

        try
        {
            mpPayment = await _paymentClient.CreateAsync(request, requestOptions);
        }
        catch (MercadoPagoApiException ex)
        {
            var apiErrorJson = ex.ApiError is not null
                ? JsonSerializer.Serialize(ex.ApiError)
                : "null";
            var responseBody = ex.ApiResponse?.Content ?? "null";
            _logger.LogError(ex,
                "Mercado Pago API error creating PIX payment. StatusCode={StatusCode}, ApiError={ApiError}, ResponseBody={ResponseBody}",
                ex.StatusCode, apiErrorJson, responseBody);
            throw;
        }

        if (mpPayment.PointOfInteraction?.TransactionData is null && mpPayment.Id.HasValue)
        {
            mpPayment = await _paymentClient.GetAsync(mpPayment.Id.Value);
        }

        var payment = new Domain.Entities.Payment
        {
            OrderId = orderId,
            Amount = amount
        };

        payment.SetPixPayment(
            mpPayment.Id.ToString(),
            mpPayment.PointOfInteraction?.TransactionData?.QrCodeBase64 ?? "",
            mpPayment.PointOfInteraction?.TransactionData?.QrCode ?? "");

        await _paymentWriteRepo.CreateAsync(payment);
        await _unitOfWork.Commit();

        return payment.Adapt<PaymentResponseDto>();
    }

    public async Task<PaymentResponseDto> CreateCardPaymentAsync(
        Guid orderId, decimal amount, string cardToken, int installments,
        string payerEmail, string? payerIdentification, string? customerId,
        string? cardPaymentMethodId = null, string? issuerId = null, string? payerIdentificationType = "CPF")
    {
        var payerRequest = new PaymentPayerRequest
        {
            Email = payerEmail
        };

        if (!string.IsNullOrEmpty(payerIdentification))
        {
            payerRequest.Identification = new IdentificationRequest
            {
                Type = payerIdentificationType ?? "CPF",
                Number = payerIdentification
            };
        }

        var request = new PaymentCreateRequest
        {
            TransactionAmount = amount,
            Token = cardToken,
            Description = $"Pedido #{orderId.ToString("N")[..8].ToUpper()}",
            Installments = installments,
            Payer = payerRequest,
            NotificationUrl = _settings.WebhookUrl
        };

        if (!string.IsNullOrEmpty(cardPaymentMethodId))
            request.PaymentMethodId = cardPaymentMethodId;

        if (!string.IsNullOrEmpty(issuerId))
            request.IssuerId = issuerId;

        var requestOptions = new MercadoPago.Client.RequestOptions();
        requestOptions.CustomHeaders["X-Idempotency-Key"] = Guid.NewGuid().ToString("N");

        _logger.LogInformation(
            "Creating card payment: amount={Amount}, installments={Installments}, methodId={MethodId}, issuerId={IssuerId}, hasCustomer={HasCustomer}, hasIdent={HasIdent}, email={Email}, tokenLen={TokenLen}",
            amount, installments, cardPaymentMethodId, issuerId,
            !string.IsNullOrEmpty(customerId), !string.IsNullOrEmpty(payerIdentification),
            payerEmail, cardToken?.Length);

        try
        {
            var requestJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                transaction_amount = amount,
                token = cardToken?[..Math.Min(20, cardToken.Length)],
                description = request.Description,
                installments = request.Installments,
                payment_method_id = request.PaymentMethodId,
                issuer_id = request.IssuerId,
                payer = new
                {
                    email = request.Payer?.Email,
                    type = request.Payer?.Type,
                    id = request.Payer?.Id,
                    identification = request.Payer?.Identification != null ? new
                    {
                        type = request.Payer.Identification.Type,
                        number = request.Payer.Identification.Number
                    } : null
                },
                notification_url = request.NotificationUrl
            });
            _logger.LogInformation("Card payment request payload: {Payload}", requestJson);

            var mpPayment = await _paymentClient.CreateAsync(request, requestOptions);

            var payment = new Domain.Entities.Payment
            {
                OrderId = orderId,
                Amount = amount
            };

            payment.SetCardPayment(
                mpPayment.Id.ToString(),
                mpPayment.Card?.LastFourDigits ?? "",
                mpPayment.Installments ?? installments,
                mpPayment.TransactionDetails?.InstallmentAmount?.ToString() ?? "");

            UpdatePaymentStatus(payment, mpPayment.Status);

            await _paymentWriteRepo.CreateAsync(payment);
            await _unitOfWork.Commit();

            return payment.Adapt<PaymentResponseDto>();
        }
        catch (MercadoPagoApiException ex)
        {
            var apiErrorJson = ex.ApiError is not null
                ? JsonSerializer.Serialize(ex.ApiError)
                : "null";
            var responseBody = ex.ApiResponse?.Content ?? "null";
            _logger.LogError(ex,
                "Mercado Pago API error creating card payment. StatusCode={StatusCode}, ApiError={ApiError}, ResponseBody={ResponseBody}",
                ex.StatusCode, apiErrorJson, responseBody);
            throw;
        }
    }

    public async Task<PaymentResponseDto> CreateBoletoPaymentAsync(
        Guid orderId, decimal amount, string payerEmail, string? payerIdentification)
    {
        var request = new PaymentCreateRequest
        {
            TransactionAmount = amount,
            PaymentMethodId = "bolbradesco",
            Description = $"Pedido #{orderId.ToString("N")[..8].ToUpper()}",
            Payer = new PaymentPayerRequest
            {
                Email = payerEmail
            },
            NotificationUrl = _settings.WebhookUrl
        };

        _logger.LogInformation(
            "Creating boleto payment: amount={Amount}, payer={Email}",
            amount, payerEmail);

        MercadoPago.Resource.Payment.Payment mpPayment;

        var requestOptions = new MercadoPago.Client.RequestOptions();
        requestOptions.CustomHeaders["X-Idempotency-Key"] = Guid.NewGuid().ToString("N");

        try
        {
            mpPayment = await _paymentClient.CreateAsync(request, requestOptions);
        }
        catch (MercadoPagoApiException ex)
        {
            var apiErrorJson = ex.ApiError is not null
                ? JsonSerializer.Serialize(ex.ApiError)
                : "null";
            var responseBody = ex.ApiResponse?.Content ?? "null";
            _logger.LogError(ex,
                "Mercado Pago API error creating boleto payment. StatusCode={StatusCode}, ApiError={ApiError}, ResponseBody={ResponseBody}",
                ex.StatusCode, apiErrorJson, responseBody);
            throw;
        }

        var payment = new Domain.Entities.Payment
        {
            OrderId = orderId,
            Amount = amount
        };

        payment.SetBoletoPayment(
            mpPayment.Id.ToString(),
            mpPayment.TransactionDetails?.ExternalResourceUrl ?? "",
            mpPayment.TransactionDetails?.Barcode?.Content ?? "");

        await _paymentWriteRepo.CreateAsync(payment);
        await _unitOfWork.Commit();

        return payment.Adapt<PaymentResponseDto>();
    }

    public async Task<PaymentResponseDto?> ProcessWebhookAsync(string payload, string? signature)
    {
        try
        {
            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;

            var dataId = root.TryGetProperty("data", out var data)
                ? data.TryGetProperty("id", out var id) ? id.GetString() : null
                : null;

            if (string.IsNullOrEmpty(dataId))
            {
                _logger.LogWarning("Webhook received without data.id: {Payload}", payload);
                return null;
            }

            var mpPayment = await _paymentClient.GetAsync(long.Parse(dataId));
            if (mpPayment == null)
            {
                _logger.LogWarning("Mercado Pago payment {Id} not found", dataId);
                return null;
            }

            var payment = await _paymentWriteRepo.GetByMercadoPagoId(dataId);
            if (payment == null)
            {
                _logger.LogWarning("Local payment for MP id {Id} not found", dataId);
                return null;
            }

            var previousStatus = payment.Status;
            UpdatePaymentStatus(payment, mpPayment.Status);

            if (previousStatus != payment.Status)
            {
                _paymentWriteRepo.Update(payment);
                await _unitOfWork.Commit();
                _logger.LogInformation(
                    "Payment {Id} status updated from {From} to {To}",
                    payment.Id, previousStatus, payment.Status);
            }

            return payment.Adapt<PaymentResponseDto>();
        }
        catch (MercadoPagoApiException ex)
        {
            _logger.LogError(ex, "Mercado Pago API error processing webhook");
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid webhook payload");
            return null;
        }
    }

    public async Task<PaymentResponseDto?> GetPaymentByMercadoPagoIdAsync(string mercadoPagoPaymentId)
    {
        var payment = await _paymentReadRepo.GetByMercadoPagoIdAsync(mercadoPagoPaymentId);
        return payment?.Adapt<PaymentResponseDto>();
    }

    private static void UpdatePaymentStatus(Domain.Entities.Payment payment, string? mpStatus)
    {
        switch (mpStatus)
        {
            case "approved":
                payment.Approve();
                break;
            case "rejected":
            case "refunded":
                payment.Reject();
                break;
            case "cancelled":
            case "charged_back":
                payment.Cancel();
                break;
        }
    }

    public async Task<string> GetOrCreateCustomerAsync(string email)
    {
        _logger.LogInformation("Searching or creating customer: email={Email}", email);

        try
        {
            var searchRequest = new MercadoPago.Client.SearchRequest
            {
                Filters = new System.Collections.Generic.Dictionary<string, object>
                {
                    { "email", email }
                }
            };

            var searchResult = await _customerClient.SearchAsync(searchRequest);
            if (searchResult.Results.Any())
            {
                return searchResult.Results.First().Id;
            }

            var customerRequest = new MercadoPago.Client.Customer.CustomerRequest
            {
                Email = email
            };

            var newCustomer = await _customerClient.CreateAsync(customerRequest);
            return newCustomer.Id;
        }
        catch (MercadoPagoApiException ex)
        {
            var apiErrorJson = ex.ApiError is not null
                ? JsonSerializer.Serialize(ex.ApiError)
                : "null";
            var responseBody = ex.ApiResponse?.Content ?? "null";
            _logger.LogError(ex,
                "Mercado Pago API error in GetOrCreateCustomerAsync. StatusCode={StatusCode}, Email={Email}, ApiError={ApiError}, ResponseBody={ResponseBody}",
                ex.StatusCode, email, apiErrorJson, responseBody);
            throw;
        }
    }

    public async Task<UserCardDto> SaveCardAsync(string customerId, string cardToken)
    {
        _logger.LogInformation(
            "Saving card for customer={CustomerId}, hasToken={HasToken}",
            customerId, !string.IsNullOrEmpty(cardToken));

        var cardRequest = new MercadoPago.Client.Customer.CustomerCardCreateRequest
        {
            Token = cardToken
        };

        MercadoPago.Resource.Customer.CustomerCard card;

        try
        {
            card = await _customerCardClient.CreateAsync(customerId, cardRequest);
        }
        catch (MercadoPagoApiException ex)
        {
            var apiErrorJson = ex.ApiError is not null
                ? JsonSerializer.Serialize(ex.ApiError)
                : "null";
            var responseBody = ex.ApiResponse?.Content ?? "null";
            _logger.LogError(ex,
                "Mercado Pago API error saving card. StatusCode={StatusCode}, CustomerId={CustomerId}, ApiError={ApiError}, ResponseBody={ResponseBody}",
                ex.StatusCode, customerId, apiErrorJson, responseBody);
            throw;
        }

        return new UserCardDto
        {
            GatewayCardId = card.Id,
            LastFourDigits = card.LastFourDigits,
            Brand = card.PaymentMethod.Name,
            ExpirationMonth = card.ExpirationMonth ?? 0,
            ExpirationYear = card.ExpirationYear ?? 0
        };
    }

}
