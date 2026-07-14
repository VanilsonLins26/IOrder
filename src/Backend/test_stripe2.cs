using Stripe;
using System;

class Program {
    static void Main() {
        var options = new PaymentIntentPaymentMethodOptionsCardOptions();
        var prop = options.GetType().GetProperty("SetupFutureUsage");
        Console.WriteLine(prop != null ? "EXISTS" : "MISSING");
    }
}
