using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using OrderProcessor.Domain.Entities;

namespace OrderProcessor.Functions.Functions
{
    public static class OrchestratorFunctions
    {
        [Function("OrderOrchestrator")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context
        )
        {
            var order = context.GetInput<Order>();
            await context.CallActivityAsync("ValidateOrder", order);
            await context.CallActivityAsync("ChargePayment", order);
            await context.CallActivityAsync("FulfillOrder", order);
        }

        [Function("ValidateOrder")]
        public static Task ValidateOrder([ActivityTrigger] Order order, FunctionContext ctx)
        {
            var log = ctx.GetLogger("ValidateOrder");
            log.LogInformation("Validating order {Id}", order.Id);
            return Task.CompletedTask;
        }

        [Function("ChargePayment")]
        public static Task ChargePayment([ActivityTrigger] Order order, FunctionContext ctx)
        {
            var log = ctx.GetLogger("ChargePayment");
            log.LogInformation("Charging payment for order {Id}", order.Id);
            return Task.CompletedTask;
        }

        [Function("FulfillOrder")]
        public static Task FulfillOrder([ActivityTrigger] Order order, FunctionContext ctx)
        {
            var log = ctx.GetLogger("FulfillOrder");
            log.LogInformation("Fulfilling order {Id}", order.Id);
            return Task.CompletedTask;
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
