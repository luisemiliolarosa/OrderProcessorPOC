using System.Threading;
using System.Threading.Tasks;
using OrderProcessor.Domain.Entities;

namespace OrderProcessor.Domain.Interfaces
{
    public interface IPaymentProcessor
    {
        Task<Payment> ChargeAsync(Payment payment);
    }
}
