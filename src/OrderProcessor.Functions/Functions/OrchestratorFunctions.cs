using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Functions.Functions
{
    public class OrderOrchestrator
    {
        private readonly IOrderProcessor _orderProcessor;
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly ILogger<OrderOrchestrator> _logger;

        public OrderOrchestrator(
            IOrderProcessor orderProcessor,
            IPaymentProcessor paymentProcessor,
            ILogger<OrderOrchestrator> logger
        )
        {
            _orderProcessor = orderProcessor;
            _paymentProcessor = paymentProcessor;
            _logger = logger;
        }

        [Function("OrderOrchestrator")]
        public async Task Run([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var order = context.GetInput<Order>();
            if (!_orderProcessor.Validate(order))
            {
                _logger.LogWarning("Order {OrderId} is invalid. Aborting orchestration.", order.Id);
                return;
            }

            // Step 1: Charge Payment
            order.Payment = await context.CallActivityAsync<Payment>(
                "ChargePaymentActivity",
                order.Payment
            );

            if (!order.Payment.IsCharged)
            {
                _logger.LogWarning(
                    "Payment failed for order {OrderId}. Aborting orchestration.",
                    order.Id
                );
                return;
            }

            // Step 2: Execute Order
            await context.CallActivityAsync("ExecuteOrderActivity", order);

            _logger.LogInformation(
                "Order {OrderId} processed successfully with Payment {PaymentId}",
                order.Id,
                order.Payment.PaymentId
            );
        }

        [Function("ChargePaymentActivity")]
        public async Task<Payment> ChargePaymentActivity([ActivityTrigger] Payment payment)
        {
            return await _paymentProcessor.ChargeAsync(payment);
        }

        [Function("ExecuteOrderActivity")]
        public async Task ExecuteOrderActivity([ActivityTrigger] Order order)
        {
            await _orderProcessor.ProcessAsync(order);
        }

        [Function("StartOrderOrchestration")]
        public static async Task<HttpResponseData> StartOrchestration(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "start")] HttpRequestData req,
            [DurableClient] IDurableClient client
        )
        {
            var order = await System.Text.Json.JsonSerializer.DeserializeAsync<Order>(req.Body);
            var instanceId = await client.StartNewAsync("OrderOrchestrator", order);
            var res = req.CreateResponse(System.Net.HttpStatusCode.Accepted);
            await res.WriteStringAsync(instanceId);
            return res;
        }
    }
}
