using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using IOrder.Application.Services.Payment;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Payment;
using IOrder.infrastructure.DataAccess;
using Mapster;
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
                Email = "test_user_br@testuser.com", // Obrigatório ser esse e-mail no sandbox para aprovar PIX
                FirstName = "APRO" // Força aprovação automática
            },
            NotificationUrl = _settings.WebhookUrl
        };

        var mpPayment = await _paymentClient.CreateAsync(request);

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
        string payerEmail, string? payerIdentification)
    {
        var request = new PaymentCreateRequest
        {
            TransactionAmount = amount,
            Token = cardToken,
            Description = $"Pedido #{orderId.ToString("N")[..8].ToUpper()}",
            Installments = installments,
            Payer = new PaymentPayerRequest
            {
                Email = payerEmail
            },
            NotificationUrl = _settings.WebhookUrl
        };

        var requestOptions = new MercadoPago.Client.RequestOptions();
        requestOptions.CustomHeaders["X-Idempotency-Key"] = Guid.NewGuid().ToString("N");

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

        await _paymentWriteRepo.CreateAsync(payment);
        await _unitOfWork.Commit();

        return payment.Adapt<PaymentResponseDto>();
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

        var mpPayment = await _paymentClient.CreateAsync(request);

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
}
