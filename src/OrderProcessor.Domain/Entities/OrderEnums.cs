namespace OrderProcessor.Domain.Entities
{
    public enum OrderType
    {
        None,
        Standard,
        Premium,
    }

    public enum OrderStatus
    {
        None,
        Pending,
        Processed,
    }
}
