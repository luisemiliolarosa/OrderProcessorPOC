using System.Threading;
using System.Threading.Tasks;
using OrderProcessor.Domain.Entities;

namespace OrderProcessor.Domain.Interfaces
{
    public interface IProcessingStrategy
    {
        Task ExecuteAsync(Order order);
    }
}
