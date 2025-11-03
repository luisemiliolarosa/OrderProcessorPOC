using OrderProcessor.Domain.Entities;

namespace OrderProcessor.Domain.Interfaces
{
    public interface IProcessingStrategyFactory
    {
        IProcessingStrategy GetStrategy(OrderType orderType);
    }
}
