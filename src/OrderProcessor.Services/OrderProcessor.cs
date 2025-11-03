using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Services
{
    public class OrderProcessor : IOrderProcessor
    {
        private readonly ILogger<OrderProcessor> _logger;
        private readonly IProcessingStrategyFactory _factory;

        public OrderProcessor(ILogger<OrderProcessor> logger, IProcessingStrategyFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        public bool Validate(Order order)
        {
            _logger.LogInformation("Validating order {OrderId}", order.Id);

            // Example: simple validation
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} validation failed", order?.Id);
                return false;
            }

            _logger.LogInformation("Order {OrderId} validated successfully", order.Id);
            return true;
        }

        public async Task ProcessAsync(Order order)
        {
            var strategy = _factory.GetStrategy(order.Type);
            await strategy.ExecuteAsync(order);
        }
    }
}
