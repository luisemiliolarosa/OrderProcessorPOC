using System.Threading;
using System.Threading.Tasks;
using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Services.Strategies
{
    public class PremiumOrderStrategy : IProcessingStrategy
    {
        public Task ExecuteAsync(Order order)
        {
            order.Status = OrderStatus.Processed;
            return Task.CompletedTask;
        }
    }
}
