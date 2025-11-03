using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OrderProcessor.Services;
using Polly;

namespace OrderProcessor.Functions.Functions
{
    public class TimerWarmUpFunction
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;

        public TimerWarmUpFunction(
            ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory
        )
        {
            _logger = loggerFactory.CreateLogger<TimerWarmUpFunction>();
            _httpClient = httpClientFactory.CreateClient("WarmUpClient");
            _retryPolicy = PollyPolicies.GetRetryPolicy();
        }

        [Function("TimerWarmUpFunction")]
        public async Task RunAsync([TimerTrigger("0 */5 * * * *")] object timer)
        {
            _logger.LogInformation("Starting warm-up cycle...");

            var warmupTargets = new[]
            {
                "api/http-order", // HTTP trigger endpoint
                "runtime/webhooks/durabletask/orchestrators/OrderOrchestrator_HttpStart", // Durable start endpoint
                "queue/order-in", // Optional: queue name (to enqueue dummy message)
            };

            foreach (var target in warmupTargets)
            {
                try
                {
                    var requestUri = BuildWarmUpUri(target);

                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        var response = await _httpClient.GetAsync(requestUri);
                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogWarning(
                                $"Warm-up ping to {target} returned {response.StatusCode}"
                            );
                        }
                        else
                        {
                            _logger.LogInformation($"Warm-up successful for {target}");
                        }

                        return response;
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error warming up {target}");
                }
            }

            _logger.LogInformation("Warm-up cycle completed.");
        }

        private static string BuildWarmUpUri(string path)
        {
            var baseUrl =
                Environment.GetEnvironmentVariable("FUNCTIONS_BASE_URL") ?? "http://localhost:7071";
            if (!path.StartsWith("/"))
                path = "/" + path;
            return $"{baseUrl.TrimEnd('/')}{path}";
        }
    }
}
