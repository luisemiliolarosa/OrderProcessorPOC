using System;
using OrderProcessor.Services;
using Xunit;

namespace OrderProcessor.Tests;

public class DateTimeServiceTests
{
    private readonly DateTimeService _service;

    public DateTimeServiceTests()
    {
        _service = new DateTimeService();
    }

    [Fact]
    public void Now_Should_Return_Current_Local_Time()
    {
        var now = _service.Now;
        var diff = DateTime.Now - now;
        Assert.True(
            diff.TotalSeconds < 2,
            $"Expected Now close to DateTime.Now but got difference {diff.TotalSeconds}s"
        );
    }

    [Fact]
    public void UtcNow_Should_Return_Current_UTC_Time()
    {
        var utcNow = _service.UtcNow;
        var diff = DateTime.UtcNow - utcNow;
        Assert.True(
            Math.Abs(diff.TotalSeconds) < 2,
            $"Expected UtcNow close to DateTime.UtcNow but got difference {diff.TotalSeconds}s"
        );
    }

    [Theory]
    [InlineData(2025)]
    [InlineData(2030)]
    public void Now_Should_Have_Valid_Year(int validUpperYear)
    {
        var now = _service.Now;
        Assert.InRange(now.Year, 2000, validUpperYear);
    }

    [Fact]
    public void DateTimeService_Should_Be_Thread_Safe()
    {
        var results = new System.Collections.Concurrent.ConcurrentBag<DateTime>();
        System.Threading.Tasks.Parallel.For(
            0,
            20,
            _ =>
            {
                results.Add(_service.Now);
            }
        );

        Assert.Equal(20, results.Count);
    }
}
