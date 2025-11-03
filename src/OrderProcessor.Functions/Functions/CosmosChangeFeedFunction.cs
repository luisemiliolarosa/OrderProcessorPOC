using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Functions.Functions
{
    public class CosmosChangeFeedFunction
    {
        private readonly IOrderProcessor _processor;
        private readonly ILogger<CosmosChangeFeedFunction> _logger;

        public CosmosChangeFeedFunction(
            IOrderProcessor processor,
            ILogger<CosmosChangeFeedFunction> logger
        )
        {
            _processor = processor;
            _logger = logger;
        }

        [Function("CosmosChangeFeed")]
        public async Task Run(
            [CosmosDBTrigger(
                databaseName: "OrdersDb",
                containerName: "Orders",
                Connection = "CosmosDBConnectionString"
            )]
                IReadOnlyList<JObject> input
        )
        {
            if (input == null || input.Count == 0)
                return;

            var orders = input
                .Select(j => j.ToObject<Order>())
                .Where(o => o != null)
                .Select(o => o!)
                .ToList();

            foreach (var order in orders)
            {
                await _processor.ProcessAsync(order, default);
            }
        }
    }
}
