using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Functions.Functions
{
    public class HttpProcessOrderFunction
    {
        private readonly IOrderProcessor _orderProcessor;
        private readonly IDateTimeService _clock;
        private readonly ILogger<HttpProcessOrderFunction> _logger;

        public HttpProcessOrderFunction(
            IOrderProcessor orderProcessor,
            IDateTimeService clock,
            ILogger<HttpProcessOrderFunction> logger
        )
        {
            _orderProcessor = orderProcessor;
            _clock = clock;
            _logger = logger;
        }

        [Function("HttpProcessOrder")]
        [QueueOutput("processed-orders", Connection = "QueueStorageConnection")]
        public async Task<OrderQueueMessage?> Run(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "post",
                Route = "process/{orderType}/{orderId}"
            )]
                HttpRequestData req,
            [CosmosDBInput(
                databaseName: "OrdersDb",
                containerName: "Orders",
                Connection = "CosmosDBConnectionString",
                Id = "{orderId}",
                PartitionKey = "{orderType}"
            )]
                Order? cosmosOrder,
            OrderType orderType,
            string orderId
        )
        {
            if (cosmosOrder == null)
            {
                var nf = req.CreateResponse(HttpStatusCode.NotFound);
                await nf.WriteStringAsync("Order not found.");
                return null;
            }

            await _orderProcessor.ProcessAsync(cosmosOrder, req.FunctionContext.CancellationToken);

            var msg = new OrderQueueMessage
            {
                OrderId = cosmosOrder.Id,
                ProcessedAt = _clock.UtcNow,
                Type = cosmosOrder.Type,
                Status = cosmosOrder.Status,
            };

            _logger.LogInformation(
                "Order {OrderId} processed at {Time}",
                msg.OrderId,
                msg.ProcessedAt
            );
            return msg;
        }
    }
}
