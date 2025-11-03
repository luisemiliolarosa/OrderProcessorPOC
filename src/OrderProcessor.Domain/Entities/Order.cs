using System;

namespace OrderProcessor.Domain.Entities
{
    public class Order
    {
        public string Id { get; set; } = default!;
        public OrderType Type { get; set; } = OrderType.Standard;
        public string CustomerName { get; set; } = default!;
        public double Amount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Payment Payment { get; set; } = new Payment();
    }
}
