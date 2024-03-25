using Application.Abstractions;

namespace Application.Services;

public class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public long UtcNowUnixTimeMilliseconds => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}