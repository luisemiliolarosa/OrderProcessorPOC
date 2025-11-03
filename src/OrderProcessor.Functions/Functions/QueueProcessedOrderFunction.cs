using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Functions.Functions
{
    public class QueueProcessedOrderFunction
    {
        private readonly IDateTimeService _clock;
        private readonly ILogger<QueueProcessedOrderFunction> _logger;

        public QueueProcessedOrderFunction(
            IDateTimeService clock,
            ILogger<QueueProcessedOrderFunction> logger
        )
        {
            _clock = clock;
            _logger = logger;
        }

        [Function("QueueProcessedOrder")]
        public void Run(
            [QueueTrigger("processed-orders", Connection = "QueueStorageConnection")]
                OrderQueueMessage message
        )
        {
            _logger.LogInformation(
                "Processed Order from Queue: {OrderId}, Type: {Type}, ProcessedAt: {ProcessedAt}, ConsumedAt: {Now}",
                message.OrderId,
                message.Type,
                message.ProcessedAt,
                _clock.UtcNow
            );
        }
    }
}
