using OnlineShop.Configuration;
using OnlineShop.Models;
using Stripe;

namespace OnlineShop.Services;

public class PaymentService
{
    public void CreatePayment(Order order)
    {
        var paymentIntent = new PaymentIntent
        {
            Id = Guid.NewGuid().ToString(),
            Amount = order.Products.Sum(x => x.Amount),
            Status = "requires_payment_method"
        };
    }

    public static void ConfigureStripe(StripePaymentConfiguration stripePaymentConfiguration)
    {
        StripeConfiguration.ApiKey = stripePaymentConfiguration.SecretKey;
    }
}