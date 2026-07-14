using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program {
    static async Task Main() {
        StripeConfiguration.ApiKey = ""sk_test_xxxxxxxxxxxxxxxxxxxx"";
        var service = new PaymentIntentService();
        
        var cusService = new CustomerService();
        var customer = await cusService.CreateAsync(new CustomerCreateOptions { Email = ""test@test.com"" });

        var createOptions = new PaymentIntentCreateOptions {
            Amount = 1000,
            Currency = ""brl"",
            PaymentMethodTypes = new List<string> { ""card"", ""boleto"" },
            Customer = customer.Id
        };
        try {
            var intent = await service.CreateAsync(createOptions);
            Console.WriteLine(""Created: "" + intent.Id);
            
            var updateOptions = new PaymentIntentUpdateOptions {
                PaymentMethodOptions = new PaymentIntentPaymentMethodOptionsOptions {
                    Card = new PaymentIntentPaymentMethodOptionsCardOptions {
                        SetupFutureUsage = ""off_session""
                    }
                }
            };
            await service.UpdateAsync(intent.Id, updateOptions);
            Console.WriteLine(""Updated successfully to off_session"");
        } catch (Exception ex) {
            Console.WriteLine(""Error: "" + ex.Message);
        }
    }
}
