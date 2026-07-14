using IOrder.Application.Services.Payment;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Payment;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using System.Text.Json;

namespace IOrder.infrastructure.Services.Payment;

public class StripePaymentService : IPaymentService
{
    private readonly StripeSettings _settings;
    private readonly IPaymentReadOnlyRepository _paymentReadOnlyRepository;
    private readonly IPaymentWriteOnlyRepository _paymentWriteOnlyRepository;
    private readonly ILogger<StripePaymentService> _logger;

    public StripePaymentService(
        IOptions<StripeSettings> settings,
        IPaymentReadOnlyRepository paymentReadOnlyRepository,
        IPaymentWriteOnlyRepository paymentWriteOnlyRepository,
        ILogger<StripePaymentService> logger)
    {
        _settings = settings.Value;
        _paymentReadOnlyRepository = paymentReadOnlyRepository;
        _paymentWriteOnlyRepository = paymentWriteOnlyRepository;
        _logger = logger;
        StripeConfiguration.ApiKey = _settings.SecretKey;
    }

    public async Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(Guid orderId, decimal amount, string? customerId)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100), // Stripe usa centavos
            Currency = "brl",
            Customer = customerId,
            Metadata = new Dictionary<string, string>
            {
                { "OrderId", orderId.ToString() }
            },
            PaymentMethodTypes = new List<string> { "card", "boleto" },
            PaymentMethodOptions = new PaymentIntentPaymentMethodOptionsOptions
            {
                Card = new PaymentIntentPaymentMethodOptionsCardOptions
                {
                    SetupFutureUsage = "off_session"
                }
            }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        var payment = new Domain.Entities.Payment
        {
            OrderId = orderId,
            Amount = amount,
            StripePaymentIntentId = paymentIntent.Id
        };

        await _paymentWriteOnlyRepository.CreateAsync(payment);

        return new PaymentIntentResponseDto
        {
            ClientSecret = paymentIntent.ClientSecret,
            PaymentIntentId = paymentIntent.Id,
            CustomerId = customerId ?? string.Empty
        };
    }

    public async Task<PaymentResponseDto?> ProcessWebhookAsync(string payload, string? signature)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(payload, signature, _settings.WebhookSecret);

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent != null)
                {
                    var payment = await _paymentWriteOnlyRepository.GetByIdTracking(Guid.Parse(paymentIntent.Metadata["OrderId"]));
                    if (payment != null)
                    {
                        if (!string.IsNullOrEmpty(paymentIntent.PaymentMethodId))
                        {
                            try
                            {
                                var pmService = new PaymentMethodService();
                                var pm = await pmService.GetAsync(paymentIntent.PaymentMethodId);
                                if (pm.Type == "boleto")
                                    payment.Method = Domain.Entities.Enums.PaymentMethod.Boleto;
                                else if (pm.Type == "card")
                                    payment.Method = Domain.Entities.Enums.PaymentMethod.CreditCard;
                                else if (pm.Type == "pix")
                                    payment.Method = Domain.Entities.Enums.PaymentMethod.Pix;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Failed to retrieve PaymentMethod type for PaymentIntent {PaymentIntentId}", paymentIntent.Id);
                            }
                        }

                        payment.Approve();
                        _paymentWriteOnlyRepository.Update(payment);
                        return MapToDto(payment);
                    }
                }
            }
            else if (stripeEvent.Type == "payment_intent.payment_failed")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent != null)
                {
                    var payment = await _paymentWriteOnlyRepository.GetByIdTracking(Guid.Parse(paymentIntent.Metadata["OrderId"]));
                    if (payment != null)
                    {
                        payment.Reject();
                        _paymentWriteOnlyRepository.Update(payment);
                        return MapToDto(payment);
                    }
                }
            }

            return null;
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Stripe webhook failed");
            return null;
        }
    }

    public async Task<PaymentResponseDto?> GetPaymentByStripeIdAsync(string stripePaymentIntentId)
    {
        var payment = await _paymentReadOnlyRepository.GetByStripeIdAsync(stripePaymentIntentId);
        return payment != null ? MapToDto(payment) : null;
    }

    public async Task<string> GetOrCreateCustomerAsync(string email, string name)
    {
        var options = new CustomerSearchOptions
        {
            Query = $"email:'{email}'",
        };
        var service = new CustomerService();
        var searchResults = await service.SearchAsync(options);
        
        if (searchResults.Data.Any())
        {
            return searchResults.Data.First().Id;
        }

        var createOptions = new CustomerCreateOptions
        {
            Email = email,
            Name = name
        };
        var customer = await service.CreateAsync(createOptions);
        return customer.Id;
    }

    public async Task DeleteCardAsync(string customerId, string paymentMethodId)
    {
        var service = new PaymentMethodService();
        await service.DetachAsync(paymentMethodId);
    }

    public async Task<List<UserCardDto>> ListCardsAsync(string customerId)
    {
        var options = new PaymentMethodListOptions
        {
            Customer = customerId,
            Type = "card",
        };
        var service = new PaymentMethodService();
        var paymentMethods = await service.ListAsync(options);

        // Deduplicate by card fingerprint
        var uniqueMethods = paymentMethods.Data
            .GroupBy(pm => pm.Card.Fingerprint)
            .Select(g => g.First())
            .ToList();

        return uniqueMethods.Select(pm => new UserCardDto
        {
            GatewayCardId = pm.Id,
            LastFourDigits = pm.Card.Last4,
            Brand = pm.Card.Brand,
            ExpirationMonth = (int)pm.Card.ExpMonth,
            ExpirationYear = (int)pm.Card.ExpYear
        }).ToList();
    }

    private PaymentResponseDto MapToDto(Domain.Entities.Payment payment)
    {
        return new PaymentResponseDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Status = (IOrder.Communication.Enums.PaymentStatusDto)payment.Status,
            Method = (IOrder.Communication.Enums.PaymentMethodDto)payment.Method,
            CreatedAt = payment.CreatedAt,
            PaidAt = payment.PaidAt,
            PixQrCode = payment.PixQrCode,
            PixCopyPaste = payment.PixCopyPaste,
            BoletoUrl = payment.BoletoUrl,
            BoletoBarcode = payment.BoletoBarcode,
            CardLastFourDigits = payment.CardLastFourDigits,
            Installments = payment.Installments,
            InstallmentAmount = payment.InstallmentAmount
        };
    }
}
