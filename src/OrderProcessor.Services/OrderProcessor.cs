using System.Threading;
using System.Threading.Tasks;
using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Services
{
    public class OrderProcessor : IOrderProcessor
    {
        private readonly IProcessingStrategyFactory _factory;

        public OrderProcessor(IProcessingStrategyFactory factory)
        {
            _factory = factory;
        }

        public async Task ProcessAsync(Order order, CancellationToken cancellationToken)
        {
            var strategy = _factory.GetStrategy(order.Type);
            await strategy.ExecuteAsync(order, cancellationToken);
        }
    }
}
