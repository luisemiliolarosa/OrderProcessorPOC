using System;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Interfaces;
using OrderProcessor.Services.Strategies;

namespace OrderProcessor.Services
{
    public class ProcessingStrategyFactory : IProcessingStrategyFactory
    {
        private readonly IServiceProvider _provider;

        public ProcessingStrategyFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IProcessingStrategy GetStrategy(OrderType orderType)
        {
            return orderType switch
            {
                OrderType.Premium => _provider.GetRequiredService<PremiumOrderStrategy>(),
                _ => _provider.GetRequiredService<StandardOrderStrategy>(),
            };
        }
    }
}
