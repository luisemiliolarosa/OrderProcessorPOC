using System;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Now => DateTime.Now;
    }
}
