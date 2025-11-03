using Azure.Identity;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderProcessor.Functions.Middleware;
using OrderProcessor.Services;

var host = new HostBuilder()
    .ConfigureAppConfiguration(
        static (context, cb) =>
        {
            cb.AddEnvironmentVariables();
            var keyVaultName = Environment.GetEnvironmentVariable("KeyVaultName");
            if (!string.IsNullOrEmpty(keyVaultName))
            {
                // Configure Azure Key Vault via DefaultAzureCredential
                var credential = new DefaultAzureCredential();
                var kvUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
                cb.AddAzureKeyVault(kvUri, credential);
            }
        }
    )
    .ConfigureFunctionsWorkerDefaults(worker =>
    {
        worker.UseMiddleware<ExceptionHandlingMiddleware>();
        worker.UseMiddleware<TelemetryMiddleware>();
    })
    .ConfigureServices(
        (context, services) =>
        {
            services.AddOrderProcessorServices(context.Configuration);
            services.AddApplicationInsightsTelemetryWorkerService();
            services.AddSingleton<ITelemetryInitializer, TelemetryEnricher>();
            services.AddSingleton<TelemetryMiddleware>();
        }
    )
    .Build();

host.Run();
