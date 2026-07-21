using IOrder.Domain.Repositories.Payment;
using IOrder.infrastructure.Services.Payment;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.Infrastructure.Payment;

public class StripePaymentServiceTest
{
    [Fact]
    public async Task ProcessWebhookAsync_InvalidPayload_ReturnsNull()
    {
        var settings = Options.Create(new StripeSettings { SecretKey = "sk_test", WebhookSecret = "whsec_test" });
        var readRepo = new Mock<IPaymentReadOnlyRepository>();
        var writeRepo = new Mock<IPaymentWriteOnlyRepository>();
        var logger = new Mock<ILogger<StripePaymentService>>();

        var service = new StripePaymentService(settings, readRepo.Object, writeRepo.Object, logger.Object);

        var result = await service.ProcessWebhookAsync("invalid_payload", "invalid_signature");
        
        result.ShouldBeNull();
    }

    [Fact]
    public void Constructor_Initializes_ApiKey()
    {
        var settings = Options.Create(new StripeSettings { SecretKey = "sk_test", WebhookSecret = "whsec_test" });
        var readRepo = new Mock<IPaymentReadOnlyRepository>();
        var writeRepo = new Mock<IPaymentWriteOnlyRepository>();
        var logger = new Mock<ILogger<StripePaymentService>>();

        var service = new StripePaymentService(settings, readRepo.Object, writeRepo.Object, logger.Object);

        Stripe.StripeConfiguration.ApiKey.ShouldBe("sk_test");
    }
}
