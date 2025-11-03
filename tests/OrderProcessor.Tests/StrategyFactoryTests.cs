using Microsoft.Extensions.DependencyInjection;
using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Interfaces;
using OrderProcessor.Services;
using OrderProcessor.Services.Strategies;
using Xunit;

namespace OrderProcessor.Tests;

public class StrategyFactoryTests
{
    private readonly IProcessingStrategyFactory _factory;

    public StrategyFactoryTests()
    {
        var services = new ServiceCollection();
        services.AddTransient<StandardOrderStrategy>();
        services.AddTransient<PremiumOrderStrategy>();
        services.AddSingleton<IProcessingStrategyFactory, ProcessingStrategyFactory>();

        var provider = services.BuildServiceProvider();
        _factory = provider.GetRequiredService<IProcessingStrategyFactory>();
    }

    [Fact]
    public void Should_Create_Standard_Strategy_By_Default()
    {
        var strategy = _factory.GetStrategy(OrderType.None);
        Assert.NotNull(strategy);
        Assert.IsType<StandardOrderStrategy>(strategy);
    }

    [Fact]
    public void Should_Create_Premium_Strategy_When_Specified()
    {
        var strategy = _factory.GetStrategy(OrderType.Premium);
        Assert.NotNull(strategy);
        Assert.IsType<PremiumOrderStrategy>(strategy);
    }

    [Theory]
    [InlineData(OrderType.Standard, typeof(StandardOrderStrategy))]
    [InlineData(OrderType.Premium, typeof(PremiumOrderStrategy))]
    public void GetStrategy_Should_Return_Correct_Type(OrderType key, System.Type expected)
    {
        var strategy = _factory.GetStrategy(key);
        Assert.IsType(expected, strategy);
    }

    [Fact]
    public void Should_Fallback_To_Default_When_Unknown_Key()
    {
        var strategy = _factory.GetStrategy(OrderType.None);
        Assert.NotNull(strategy);
        Assert.IsType<StandardOrderStrategy>(strategy);
    }
}
