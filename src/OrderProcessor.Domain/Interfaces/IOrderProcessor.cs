using System.Threading;
using System.Threading.Tasks;
using OrderProcessor.Domain.Entities;

namespace OrderProcessor.Domain.Interfaces
{
    public interface IOrderProcessor
    {
        Task ProcessAsync(Order order, CancellationToken cancellationToken);
    }
}
