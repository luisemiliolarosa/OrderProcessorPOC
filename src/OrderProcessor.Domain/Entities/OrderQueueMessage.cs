using System;

namespace OrderProcessor.Domain.Entities
{
    public class OrderQueueMessage
    {
        public string OrderId { get; set; } = default!;
        public DateTime ProcessedAt { get; set; }
        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }
    }
}
