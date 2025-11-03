using System;
using System.Net.Http;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace OrderProcessor.Services
{
    public static class PollyPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            // Exponential backoff with decorrelated jitter (Polly.Contrib)
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(msg => (int)msg.StatusCode >= 500)
                .WaitAndRetryAsync(5, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(msg => (int)msg.StatusCode >= 500)
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(60)
                );
        }
    }
}
