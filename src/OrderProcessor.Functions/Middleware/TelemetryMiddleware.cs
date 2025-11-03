using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace OrderProcessor.Functions.Middleware
{
    public class TelemetryMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly TelemetryClient _telemetry;

        public TelemetryMiddleware(TelemetryClient telemetry)
        {
            _telemetry = telemetry;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var activity = new Activity("FunctionExecution");
            activity.Start();

            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                throw;
            }
            finally
            {
                activity.Stop();
                _telemetry.TrackEvent(
                    "FunctionCompleted",
                    new Dictionary<string, string?>
                    {
                        ["FunctionName"] = context.FunctionDefinition.Name,
                        ["TraceId"] = activity.TraceId.ToString(),
                    }
                );
            }
        }
    }
}
