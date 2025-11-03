namespace OrderProcessor.Domain.Entities
{
    public class Payment
    {
        public string PaymentId { get; set; } = Guid.NewGuid().ToString();
        public decimal Amount { get; set; }
        public bool IsCharged { get; set; } = false;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    }
}
