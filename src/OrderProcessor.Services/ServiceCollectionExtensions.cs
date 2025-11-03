using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Interfaces;
using OrderProcessor.Services.Strategies;
using Polly;

namespace OrderProcessor.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrderProcessorServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // strategies
            services.AddTransient<StandardOrderStrategy>();
            services.AddTransient<PremiumOrderStrategy>();

            services.AddTransient<IPaymentProcessor, PaymentProcessor>();
            services.AddSingleton<IOrderProcessor, OrderProcessor>();

            services.AddSingleton<IProcessingStrategyFactory, ProcessingStrategyFactory>();
            services.AddSingleton<IDateTimeService, DateTimeService>();

            // Feature flag provider via KeyVault (optional)
            var keyVaultName = configuration["KeyVaultName"];
            if (!string.IsNullOrWhiteSpace(keyVaultName))
            {
                services.AddSingleton<IFeatureFlagProvider>(_ => new KeyVaultFeatureFlagProvider(
                    keyVaultName
                ));
            }

            // HttpClient with Polly policies
            services
                .AddHttpClient("ResilientClient")
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler
                    {
                        AutomaticDecompression =
                            DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    }
                )
                .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
                .AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy());

            return services;
        }
    }
}
