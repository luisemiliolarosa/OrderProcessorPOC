using System;

namespace OrderProcessor.Domain.Interfaces
{
    public interface IDateTimeService
    {
        DateTime UtcNow { get; }
        DateTime Now { get; }
    }
}
