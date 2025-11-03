# OrderProcessorPOC (Isolated Worker)

This solution is a POC Azure Functions app using the **isolated worker model**.
It includes:
- Cosmos DB Change Feed trigger
- Azure Queue Trigger
- HTTP Trigger (Cosmos input + Queue output)
- Timer trigger to keep things warm
- Durable Functions (orchestration + activity functions)
- Exception middleware and correlation middleware
- Application Insights wiring, Key Vault feature flags, Polly policies for resilience
- DateTimeService injected via DI
- Strategy/Factory/Iterator patterns in Services layer
- Unit tests (xUnit)

## Configuration (local.settings.json)

Place a `local.settings.json` in `src/OrderProcessor.Functions`:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "CosmosDBConnectionString": "<COSMOS_CONN>",
    "QueueStorageConnection": "<STORAGE_CONN>",
    "APPINSIGHTS_CONNECTIONSTRING": "<APPINSIGHTS_CONN>",
    "KeyVaultName": "<KEYVAULT_NAME>"
  }
}
```

## Build & Run
1. `dotnet restore`
2. `dotnet build`
3. `cd src/OrderProcessor.Functions`
4. `func start` (requires Azure Functions Core Tools v4)

