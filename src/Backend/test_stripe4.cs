using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program {
    static async Task Main() {
        StripeConfiguration.ApiKey = "sk_test_xxxxxxxxxxxxxxxxxxxx";
        var service = new PaymentIntentService();
        var createOptions = new PaymentIntentCreateOptions {
            Amount = 1000,
            Currency = "brl",
            PaymentMethodTypes = new List<string> { "card", "boleto" },
            Customer = "cus_R0GshXg09Y9q0G" // Just using any customer if needed, but not strictly needed for this test unless we add off_session
        };
        try {
            var intent = await service.CreateAsync(createOptions);
            Console.WriteLine("Created: " + intent.Id);
            
            var updateOptions = new PaymentIntentUpdateOptions {
                PaymentMethodOptions = new PaymentIntentPaymentMethodOptionsOptions {
                    Card = new PaymentIntentPaymentMethodOptionsCardOptions {
                        SetupFutureUsage = "off_session"
                    }
                }
            };
            await service.UpdateAsync(intent.Id, updateOptions);
            Console.WriteLine("Updated successfully to off_session");
        } catch (Exception ex) {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
