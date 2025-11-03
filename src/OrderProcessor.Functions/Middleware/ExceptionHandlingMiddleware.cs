using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace OrderProcessor.Functions.Middleware
{
    public class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly TelemetryClient _telemetry;

        public ExceptionHandlingMiddleware(
            ILogger<ExceptionHandlingMiddleware> logger,
            TelemetryClient telemetry
        )
        {
            _logger = logger;
            _telemetry = telemetry;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception in function {FunctionName}",
                    context.FunctionDefinition.Name
                );
                _telemetry.TrackException(
                    ex,
                    properties: new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "FunctionName", context.FunctionDefinition.Name },
                    }
                );
                // if HTTP trigger, try to write a response
                var req = await context.GetHttpRequestDataAsync();
                if (req != null)
                {
                    var response = req.CreateResponse(
                        System.Net.HttpStatusCode.InternalServerError
                    );
                    await response.WriteStringAsync("Internal server error. See logs for details.");
                    context.GetInvocationResult().Value = response;
                    return;
                }
                throw; // rethrow for non-HTTP triggers so runtime applies retry/poison handling
            }
        }
    }
}
