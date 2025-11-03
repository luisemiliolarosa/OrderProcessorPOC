using System.Diagnostics;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace OrderProcessor.Functions.Middleware
{
    public class TelemetryEnricher : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            var currentActivity = Activity.Current;
            if (currentActivity != null)
            {
                telemetry.Context.Operation.Id = currentActivity.RootId;
                telemetry.Context.Operation.ParentId = currentActivity.ParentId;
                telemetry.Context.GlobalProperties["CorrelationId"] =
                    currentActivity.TraceId.ToString();
            }
        }
    }
}
