using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Services
{
    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly ILogger<PaymentProcessor> _logger;

        public PaymentProcessor(ILogger<PaymentProcessor> logger)
        {
            _logger = logger;
        }

        public Task<Payment> ChargeAsync(Payment payment)
        {
            _logger.LogInformation(
                "Charging payment {PaymentId} amount {Amount}",
                payment.PaymentId,
                payment.Amount
            );

            // Simulate charge logic
            payment.IsCharged = true;
            payment.Status = PaymentStatus.Charged;

            return Task.FromResult(payment);
        }
    }
}
